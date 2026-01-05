using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Utilidades;

namespace SistemaTaskWhatsapp.Controllers
{
    public class EvidenciasController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public EvidenciasController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var listaEvidencias = await _contenedorTrabajo.Evidencia.GetAllAsync();

            return View(listaEvidencias);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Evidencia model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existeNombre = await _contenedorTrabajo.Evidencia.GetFirstOrDefaultAsync(e => e.Id == model.Id);

            if (existeNombre != null)
            {
                ModelState.AddModelError("", "Ya existe una evidencia con ese id");
                return View(model);
            }

            await _contenedorTrabajo.Evidencia.AddAsync(model);
            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _contenedorTrabajo.Evidencia.GetByIdAsync(id);

            if (model == null) return RedirectToAction("Index");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Evidencia model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _contenedorTrabajo.Evidencia.Update(model);
            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var EvidenciaEliminar = await _contenedorTrabajo.Evidencia.GetByIdAsync(id);

            if (EvidenciaEliminar == null) return RedirectToAction("Index");

            _contenedorTrabajo.Evidencia.Remove(EvidenciaEliminar);
            await _contenedorTrabajo.SaveAsync();
            return RedirectToAction("Index");

        }
    }
}
