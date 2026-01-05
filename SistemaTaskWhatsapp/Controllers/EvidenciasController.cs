using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Models.ViewModels;
using SistemaTaskWhatsapp.Utilidades;
using System.Threading.Tasks;

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
            var listaEvidencias = await _contenedorTrabajo.Evidencia.GetAllAsync(includeProperties: "Reporte");
            return View(listaEvidencias);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new EvidenciaVM
            {
                ListaReportes = await _contenedorTrabajo.Reporte.ListaReportes()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EvidenciaVM model)
        {
            if (!ModelState.IsValid)
            {
                model.ListaReportes = await _contenedorTrabajo.Reporte.ListaReportes();
                return View(model);
            }

            var existeNombre = await _contenedorTrabajo.Evidencia.GetFirstOrDefaultAsync(e => e.Id == model.Evidencia.Id);

            if (existeNombre != null)
            {
                ModelState.AddModelError("", "Ya existe una evidencia con ese id");
                model.ListaReportes = await _contenedorTrabajo.Reporte.ListaReportes();
                return View(model);
            }

            await _contenedorTrabajo.Evidencia.AddAsync(model.Evidencia);
            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var evidencia = await _contenedorTrabajo.Evidencia.GetByIdAsync(id);

            if (evidencia == null)
            {
                return RedirectToAction("Index");
            }

            var model = new EvidenciaVM
            {
                Evidencia = evidencia,
                ListaReportes = await _contenedorTrabajo.Reporte.ListaReportes()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EvidenciaVM model)
        {
            if (!ModelState.IsValid)
            {
                model.ListaReportes = await _contenedorTrabajo.Reporte.ListaReportes();
                return View(model);
            }

            _contenedorTrabajo.Evidencia.Update(model.Evidencia);
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
