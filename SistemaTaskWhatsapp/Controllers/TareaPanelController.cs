using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
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
            var lista = await _contenedorTrabajo.Tarea.GetAllAsync(t => t.Proyecto.Id == id, includeProperties: "Proyecto,TareaEmpleados");

            var model = new TareaPanelVM
            {
                Lista = lista.Select(x => new TareaCardVM
                {
                    Tarea = x,
                    ReportesPendientesRevisar = _contenedorTrabajo.Reporte.
                    GetAllQueryable(r => r.TareaId == x.Id && r.Estado == EstadosReporte.PendienteRevisar).
                    Count()
                }),
                Tarea = new Tarea()
                {
                    FechaEntrega = DateTime.Now.AddDays(1)
                },
                ProyectoId = id
            };

            ViewBag.AbrirModalCrear = false;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TareaPanelVM model)
        {
            if (!ModelState.IsValid)
            {
                var lista = await _contenedorTrabajo.Tarea.GetAllAsync(t => t.Proyecto.Id == model.ProyectoId, includeProperties: "Proyecto,TareaEmpleados");

                var vm = new TareaPanelVM
                {
                    Lista = lista.Select(x => new TareaCardVM
                        {
                            Tarea = x,
                            ReportesPendientesRevisar = _contenedorTrabajo.Reporte.
                            GetAllQueryable(r => r.TareaId == x.Id && r.Estado == EstadosReporte.PendienteRevisar).
                            Count()
                        }),
                    Tarea = model.Tarea,
                    ProyectoId = model.Tarea.ProyectoId
                };

                ViewBag.AbrirModalCrear = true;
                return View("Index", vm);
            }

            if (model.Tarea.FechaEntrega < DateTime.Now)
            {
                ModelState.AddModelError("Tarea.FechaEntrega", "La fecha que intenta ingresar no es valida");

                var lista = await _contenedorTrabajo.Tarea.GetAllAsync(t => t.Proyecto.Id == model.ProyectoId, includeProperties: "Proyecto,TareaEmpleados");

                var vm = new TareaPanelVM
                {
                    Lista = lista.Select(x => new TareaCardVM
                    {
                        Tarea = x,
                        ReportesPendientesRevisar = _contenedorTrabajo.Reporte.
                            GetAllQueryable(r => r.TareaId == x.Id && r.Estado == EstadosReporte.PendienteRevisar).
                            Count()
                    }),
                    Tarea = model.Tarea,
                    ProyectoId = model.Tarea.ProyectoId
                };

                ViewBag.AbrirModalCrear = true;
                return View("Index", vm);
            }

            model.Tarea.FechaInicio = DateTime.Now;

            await _contenedorTrabajo.Tarea.AddAsync(model.Tarea);
            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index", new {id = model.Tarea.ProyectoId });
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
