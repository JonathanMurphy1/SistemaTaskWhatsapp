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
            var listaEmpresas = await _contenedorTrabajo.Empresa.GetAllAsync(includeProperties: "Programa");

            return View(listaEmpresas);
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
            if(!ModelState.IsValid)
            {
                model.ListaProgramas = await _contenedorTrabajo.Programa.GetProgramaDropdown();
                return View(model);
            }

            var existeNombre = await _contenedorTrabajo.Empresa
                                    .GetFirstOrDefaultAsync(e =>
                                        e.Nombre == model.Empresa.Nombre &&
                                        e.ProgramaId == model.Empresa.ProgramaId
                                    );

            if (existeNombre != null)
            {
                ModelState.AddModelError("", "Ya existe una empresa con ese nombre");
                model.ListaProgramas = await _contenedorTrabajo.Programa.GetProgramaDropdown();
                return View(model);
            }

            model.Empresa.FechaRegistro = DateTime.Now;

            await _contenedorTrabajo.Empresa.AddAsync(model.Empresa);
            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var empresa = await _contenedorTrabajo.Empresa.GetByIdAsync(id);

            if (empresa == null) return RedirectToAction("Index");

            var model = new EmpresaVM
            {
                Empresa = empresa,
                ListaProgramas = await _contenedorTrabajo.Programa.GetProgramaDropdown()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EmpresaVM model)
        {
            if (!ModelState.IsValid)
            {
                model.ListaProgramas = await _contenedorTrabajo.Programa.GetProgramaDropdown();
                return View(model);
            }

            var existeNombre = await _contenedorTrabajo.Empresa.GetFirstOrDefaultAsync(e => e.Nombre == model.Empresa.Nombre && e.Id != model.Empresa.Id);

            if (existeNombre != null)
            {
                ModelState.AddModelError("", "Ya existe una empresa con ese nombre");
                model.ListaProgramas = await _contenedorTrabajo.Programa.GetProgramaDropdown();
                return View(model);
            }

            _contenedorTrabajo.Empresa.Update(model.Empresa);
            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var empresaEliminar = await _contenedorTrabajo.Empresa.GetFirstOrDefaultAsync(e => e.Id == id, includeProperties:"Proyectos");

            if(empresaEliminar == null) return RedirectToAction("Index");

            _contenedorTrabajo.Empresa.Remove(empresaEliminar);
            await _contenedorTrabajo.SaveAsync();
            return RedirectToAction("Index");
        }
    }
}
