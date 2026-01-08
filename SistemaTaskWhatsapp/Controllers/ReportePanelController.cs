using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
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
            var lista = await _contenedorTrabajo.Reporte.GetAllAsync(r => r.TareaId == id, includeProperties: "Tarea,Evidencias,Empleado.Usuario");
            
            lista = lista.OrderByDescending(r => r.Estado == EstadosReporte.PendienteRevisar)
                .ThenBy(r => r.FechaSubida).ToList();
            
            var tarea = await _contenedorTrabajo.Tarea.GetByIdAsync(id);

            ViewBag.ProyectoId = tarea.ProyectoId;
            ViewBag.TareaId = tarea.Id;

            return View(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Revisar(RevisarVM model)
        {
            if (!ModelState.IsValid) return RedirectToAction("Index", new { id = model.TareaId });

            var retroalimentacion = new Retroalimentacion
            {
                Comentario = model.Comentario,
                Fecha = DateTime.Now,
                SupervisorId = null,
                ReporteId = model.ReporteId,
            };

            await _contenedorTrabajo.Retroalimentacion.AddAsync(retroalimentacion);

            var reporte = await _contenedorTrabajo.Reporte.GetByIdAsync(model.ReporteId);
            reporte.Estado = model.EstadoReporte;

            await _contenedorTrabajo.SaveAsync();
            return RedirectToAction("Index", new { id = model.TareaId });
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerRevision(int reporteId)
        {
            var revision = await _contenedorTrabajo.Retroalimentacion.GetFirstOrDefaultAsync(r => r.Id == reporteId, includeProperties:"Reporte,Supervisor");

            if(revision == null) return NotFound();

            var datos = new
            {
                Comentario = revision.Comentario,
                Estado = revision.Reporte.Estado,
                Fecha = revision.Fecha.ToString("dd/MM/yyyy"),
                Supervisor = revision.Supervisor?.Nombre ?? "Sin supervisor",
                RevisionId = revision.Id,
            };

            return Json(datos);
        }

        [HttpGet]
        public async Task<IActionResult> EvidenciasReporte(int reporteId)
        {
            var evidencias = await _contenedorTrabajo.Evidencia.GetAllAsync(e => e.ReporteId == reporteId, includeProperties: "Reporte");

            if (evidencias == null) return NotFound();

            var datos = evidencias.Select(e => new
            {
                descripcion = e.Descripcion,
                url = e.Url,
            });

            return Json(datos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarRevision(RevisarVM model)
        {
            if (!ModelState.IsValid || model.RevisionId == null) return RedirectToAction("Index", new { id = model.TareaId });

            var retroalimentacionBd = await _contenedorTrabajo.Retroalimentacion.GetByIdAsync((int)model.RevisionId);

            retroalimentacionBd.Comentario = model.Comentario;
            retroalimentacionBd.Fecha = DateTime.Now;

            var reporteBd = await _contenedorTrabajo.Reporte.GetByIdAsync(model.ReporteId);
            reporteBd.Estado = model.EstadoReporte;

            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index", new {id =  model.TareaId});
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
