using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;

namespace SistemaTaskWhatsapp.Controllers
{
    public class SupervisoresController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public SupervisoresController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var listaEmpresas = await _contenedorTrabajo.Supervisor.GetAllAsync();

            return View(listaEmpresas);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Supervisor model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existeNombre = await _contenedorTrabajo.Supervisor.GetFirstOrDefaultAsync(s => s.Id == model.Id);

            if (existeNombre != null)
            {
                ModelState.AddModelError("", "Ya existe un supervisor con ese id");
                return View(model);
            }

            await _contenedorTrabajo.Supervisor.AddAsync(model);
            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _contenedorTrabajo.Supervisor.GetByIdAsync(id);

            if (model == null) return RedirectToAction("Index");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Supervisor model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _contenedorTrabajo.Supervisor.Update(model);
            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var supervisorEliminar = await _contenedorTrabajo.Supervisor.GetByIdAsync(id);

            if (supervisorEliminar == null) return RedirectToAction("Index");

            _contenedorTrabajo.Supervisor.Remove(supervisorEliminar);
            await _contenedorTrabajo.SaveAsync();
            return RedirectToAction("Index");

        }

    }
}
