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
        public async Task<IActionResult> GetProgramasByEmpresa(int id)
        {
            var empresa = await _contenedorTrabajo.Empresa.GetByIdAsync(id);

            if (empresa == null)
                return NotFound();

            var lista = await _contenedorTrabajo.EmpresaPrograma
                .GetAllAsync(
                    x => x.EmpresaId == id,
                    includeProperties: "Programa"
                );

            var result = lista.Select(x => new
            {
                id = x.Id,
                nombre = x.Programa.Nombre,
                puedeEliminar = empresa.ProgramaOrigenId != x.ProgramaId
            });

            return Json(result);
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
        public async Task<IActionResult> GetProgramas()
        {
            var lista = await _contenedorTrabajo.Programa.GetAllAsync();

            return Json(lista.Select(x => new {
                id = x.Id,
                nombre = x.Nombre
            }));
        }

        [HttpPost]
        public async Task<IActionResult> AddRelacion(int empresaId, int programaId)
        {
            var existe = await _contenedorTrabajo.EmpresaPrograma
                .GetFirstOrDefaultAsync(x => x.EmpresaId == empresaId && x.ProgramaId == programaId);

            if (existe != null)
                return BadRequest("Ya existe la relación");

            var relacion = new EmpresaPrograma
            {
                EmpresaId = empresaId,
                ProgramaId = programaId
            };

            await _contenedorTrabajo.EmpresaPrograma.AddAsync(relacion);
            await _contenedorTrabajo.SaveAsync();

            return Ok();
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

            //Solicitar borrar mensajes antes que otra cosa
            var tieneMensajes = await _contenedorTrabajo.Mensaje
                               .GetFirstOrDefaultAsync(x => x.EmpresaId == id);

            if (tieneMensajes != null)
            {
                TempData["Error"] = "No se puede eliminar la empresa porque tiene mensajes asociados";
                return RedirectToAction("Index");
            }

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
        public async Task<IActionResult> DeleteRelacionAjax(int id)
        {
            var relacion = await _contenedorTrabajo.EmpresaPrograma.GetByIdAsync(id);

            if (relacion == null)
                return NotFound();

            var empresa = await _contenedorTrabajo.Empresa
                .GetByIdAsync(relacion.EmpresaId);

            if (empresa == null)
                return NotFound();

            if (relacion.Empresa.ProgramaOrigenId == relacion.ProgramaId)
            {
                return BadRequest("No se puede eliminar el programa origen");
            }

            _contenedorTrabajo.EmpresaPrograma.Remove(relacion);
            await _contenedorTrabajo.SaveAsync();

            return Ok();
        }

    }
}
