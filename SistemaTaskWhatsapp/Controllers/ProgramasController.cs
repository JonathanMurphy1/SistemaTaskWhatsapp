using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ProgramasController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public ProgramasController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var listaProgramas = await _contenedorTrabajo.Programa.GetAllAsync();

            return View(listaProgramas);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Programa model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existeNombre = await _contenedorTrabajo.Programa.GetFirstOrDefaultAsync(p => p.Nombre == model.Nombre);

            if (existeNombre != null)
            {
                ModelState.AddModelError("", "Ya existe un programa con ese nombre");
                return View(model);
            }

            model.FechaRegistro = DateTime.Now;

            await _contenedorTrabajo.Programa.AddAsync(model);
            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _contenedorTrabajo.Programa.GetByIdAsync(id);

            if (model == null) return RedirectToAction("Index");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Programa model)
        {

            if (model.Id == 1)
            {
                TempData["Error"] = "No se puede editar el programa principal";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existeNombre = await _contenedorTrabajo.Programa.GetFirstOrDefaultAsync(p => p.Nombre == model.Nombre && p.Id != model.Id);

            if (existeNombre != null)
            {
                ModelState.AddModelError("", "Ya existe un programa con ese nombre");
                return View(model);
            }

            _contenedorTrabajo.Programa.Update(model);
            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 1)
            {
                TempData["Error"] = "No se puede eliminar el programa principal";
                return RedirectToAction("Index");
            }

            var programasEliminar = await _contenedorTrabajo.Programa.GetFirstOrDefaultAsync(p => p.Id == id);

            if(programasEliminar == null) return RedirectToAction("Index");


            _contenedorTrabajo.Programa.Remove(programasEliminar);
            await _contenedorTrabajo.SaveAsync();
            return RedirectToAction("Index");
        }
    }
}
