using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models.ViewModels;
using SistemaTaskWhatsapp.Utilidades;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Controllers
{
    public class ReportePanelController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public ReportePanelController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            var lista = await _contenedorTrabajo.Reporte.GetAllAsync(r => r.TareaId == id, includeProperties: "Tarea,Evidencias");
            var tarea = await _contenedorTrabajo.Tarea.GetByIdAsync(id);

            ViewBag.ProyectoId = tarea.ProyectoId; 

            return View(lista);
        }

        //[HttpGet]
        //public async Task<IActionResult> Create()
        //{
        //    var model = new ReporteVM
        //    {
        //        ListaTareas = await _contenedorTrabajo.Tarea.ObtenerTareasVigentes(),
        //    };

        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Create(ReporteVM model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        model.ListaTareas = await _contenedorTrabajo.Tarea.ObtenerTareasVigentes();
        //        return View(model);
        //    }

        //    model.Reporte.FechaSubida = DateTime.Now;

        //    await _contenedorTrabajo.Reporte.AddAsync(model.Reporte);
        //    await _contenedorTrabajo.SaveAsync();

        //    return RedirectToAction("Index");
        //}

        //[HttpGet]
        //public async Task<IActionResult> Edit(int id)
        //{
        //    var reporte = await _contenedorTrabajo.Reporte.GetByIdAsync(id);

        //    if (reporte == null) return RedirectToAction("Index");

        //    var model = new ReporteVM
        //    {
        //        Reporte = reporte,
        //        ListaTareas = await _contenedorTrabajo.Tarea.ObtenerTareasVigentes(),
        //    };

        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Edit(ReporteVM model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        model.ListaTareas = await _contenedorTrabajo.Tarea.ObtenerTareasVigentes();
        //        return View(model);
        //    }

        //    _contenedorTrabajo.Reporte.Update(model.Reporte);
        //    await _contenedorTrabajo.SaveAsync();

        //    return RedirectToAction("Index");
        //}

        //[HttpPost]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var reporte = await _contenedorTrabajo.Reporte.GetByIdAsync(id);

        //    if (reporte == null) return RedirectToAction("Index");

        //    _contenedorTrabajo.Reporte.Remove(reporte);
        //    await _contenedorTrabajo.SaveAsync();

        //    return RedirectToAction("Index");
        //}
    }
}
