using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Models.ViewModels;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class EmpresasController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public EmpresasController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo; 
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var empresas = await _contenedorTrabajo.Empresa.GetAllAsync(
                includeProperties: "ProgramaOrigen,EmpresaProgramas,EmpresaProgramas.Programa"
            );

            return View(empresas);
        }

        [HttpGet]
        public async Task<IActionResult> CreateAsync()
        {
            var model = new EmpresaVM
            {
                ListaProgramas = await _contenedorTrabajo.Programa.GetProgramaDropdown()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmpresaVM model)
        {
            if (!ModelState.IsValid)
            {
                model.ListaProgramas = await _contenedorTrabajo.Programa.GetProgramaDropdown();
                return View(model);
            }

            //Validar duplicado por nombre
            var existe = await _contenedorTrabajo.Empresa
                .GetFirstOrDefaultAsync(e =>
                    e.Nombre.ToLower() == model.Empresa.Nombre.ToLower());

            if (existe != null)
            {
                ModelState.AddModelError("", "Ya existe una empresa con ese nombre");
                model.ListaProgramas = await _contenedorTrabajo.Programa.GetProgramaDropdown();
                return View(model);
            }

            //Crear empresa
            model.Empresa.FechaRegistro = DateTime.Now;
            model.Empresa.ProgramaOrigenId = 1; //Por defecto de mientras ya que todos las empresas vienen de TW

            await _contenedorTrabajo.Empresa.AddAsync(model.Empresa);
            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var empresa = await _contenedorTrabajo.Empresa.GetByIdAsync(id);

            if (empresa == null)
                return RedirectToAction("Index");

            var model = new EmpresaVM
            {
                Empresa = empresa,
                ListaProgramas = await _contenedorTrabajo.Programa.GetProgramaDropdown()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EmpresaVM model)
        {
            if (!ModelState.IsValid)
            {
                model.ListaProgramas = await _contenedorTrabajo.Programa.GetProgramaDropdown();
                return View(model);
            }

            var empresa = await _contenedorTrabajo.Empresa.GetByIdAsync(model.Empresa.Id);

            if (empresa == null)
                return RedirectToAction("Index");

            // Validar permisos
            //if (empresa.ProgramaOrigenId != model.ProgramaId)
            //{
            //    ModelState.AddModelError("", "No tienes permisos para editar esta empresa");
            //                            model.ListaProgramas = await _contenedorTrabajo.Programa.GetProgramaDropdown();
            //                            return View(model);
            //}

            var existeNombre = await _contenedorTrabajo.Empresa
                                    .GetFirstOrDefaultAsync(e => e.Nombre == 
                                    model.Empresa.Nombre && e.Id != empresa.Id);

            if (existeNombre != null)
            {
                ModelState.AddModelError("", "Ya existe una empresa con ese nombre");
                model.ListaProgramas = await _contenedorTrabajo.Programa.GetProgramaDropdown();
                return View(model);
            }

            empresa.Nombre = model.Empresa.Nombre;

            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var empresa = await _contenedorTrabajo.Empresa
                .GetByIdAsync(id);

            if (empresa == null)
                return RedirectToAction("Index");

            //eliminar relaciones primero
            var relaciones = await _contenedorTrabajo.EmpresaPrograma
                .GetAllAsync(x => x.EmpresaId == id);

            foreach (var rel in relaciones)
            {
                _contenedorTrabajo.EmpresaPrograma.Remove(rel);
            }

            //eliminar empresa
            _contenedorTrabajo.Empresa.Remove(empresa);

            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRelacion(int id)
        {
            var relacion = await _contenedorTrabajo.EmpresaPrograma
                .GetByIdAsync(id);

            if (relacion == null)
                return RedirectToAction("Index");

            _contenedorTrabajo.EmpresaPrograma.Remove(relacion);
            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index");
        }


    }
}
