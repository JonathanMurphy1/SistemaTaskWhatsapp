using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models.ViewModels;
using SistemaTaskWhatsapp.Utilidades;

namespace SistemaTaskWhatsapp.Controllers
{
    public class TareaPanelController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public TareaPanelController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            var lista = await _contenedorTrabajo.Tarea.GetAllAsync(t => t.Proyecto.Id == id, includeProperties: "Proyecto");

            var model = lista.Select(x => new TareaPanelVM
            {
                Tarea = x,
                ReportesPendientesRevisar = _contenedorTrabajo.Reporte.
                    GetAllQueryable(r => r.TareaId == x.Id && r.Estado == EstadosReporte.PendienteRevisar).
                    Count()
            });

            return View(model);
        }

        //[HttpGet]
        //public async Task<IActionResult> Create()
        //{
        //    var model = new TareaVM
        //    {
        //        ListaProyectos = await _contenedorTrabajo.Proyecto.ObtenerProyectosVigentes(),
        //    };

        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Create(TareaVM model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        model.ListaProyectos = await _contenedorTrabajo.Proyecto.ObtenerProyectosVigentes();
        //        return View(model);
        //    }

        //    if (model.Tarea.FechaEntrega < DateTime.Now)
        //    {
        //        ModelState.AddModelError("Tarea.Fechaentrega", "La fecha de entrega debe ser mayor a la actual");
        //        model.ListaProyectos = await _contenedorTrabajo.Proyecto.ObtenerProyectosVigentes();
        //        return View(model);
        //    }

        //    model.Tarea.FechaInicio = DateTime.Now;

        //    await _contenedorTrabajo.Tarea.AddAsync(model.Tarea);
        //    await _contenedorTrabajo.SaveAsync();

        //    return RedirectToAction("Index");
        //}

        //[HttpGet]
        //public async Task<IActionResult> Edit(int id)
        //{
        //    var tarea = await _contenedorTrabajo.Tarea.GetByIdAsync(id);

        //    if (tarea == null) return RedirectToAction("Index");

        //    var model = new TareaVM
        //    {
        //        Tarea = tarea,
        //        ListaProyectos = await _contenedorTrabajo.Proyecto.ObtenerProyectosVigentes(),
        //    };

        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Edit(TareaVM model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        model.ListaProyectos = await _contenedorTrabajo.Proyecto.ObtenerProyectosVigentes();
        //        return View(model);
        //    }

        //    if (model.Tarea.FechaEntrega < DateTime.Now)
        //    {
        //        ModelState.AddModelError("Tarea.Fechaentrega", "La fecha de entrega debe ser mayor a la actual");
        //        model.ListaProyectos = await _contenedorTrabajo.Proyecto.ObtenerProyectosVigentes();
        //        return View(model);
        //    }

        //    if (model.Tarea.Estado == EstadosTarea.Finalizada)
        //    {
        //        model.Tarea.FechaTermino = DateTime.Now;
        //    }

        //    _contenedorTrabajo.Tarea.Update(model.Tarea);
        //    await _contenedorTrabajo.SaveAsync();

        //    return RedirectToAction("Index");
        //}

        //[HttpPost]
        //public async Task<IActionResult> Finalizar(int id)
        //{
        //    var tarea = await _contenedorTrabajo.Tarea.GetByIdAsync(id);

        //    if (tarea == null) return RedirectToAction("Index");

        //    tarea.Estado = EstadosTarea.Finalizada;
        //    tarea.FechaTermino = DateTime.Now;

        //    await _contenedorTrabajo.SaveAsync();

        //    return RedirectToAction("Index");
        //}

        //[HttpPost]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var tarea = await _contenedorTrabajo.Tarea.GetByIdAsync(id);

        //    if (tarea == null) return RedirectToAction("Index");

        //    _contenedorTrabajo.Tarea.Remove(tarea);
        //    await _contenedorTrabajo.SaveAsync();

        //    return RedirectToAction("Index");
        //}
    }
}
