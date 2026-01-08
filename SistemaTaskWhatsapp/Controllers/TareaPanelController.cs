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

            lista = lista.OrderByDescending(t => t.Estado == EstadosTarea.Pendiente)
                .ThenBy(t => t.FechaEntrega).ToList();

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

                lista = lista.OrderByDescending(t => t.Estado == EstadosTarea.Pendiente)
                    .ThenBy(t => t.FechaEntrega).ToList();


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

                lista = lista.OrderByDescending(t => t.Estado == EstadosTarea.Pendiente)
                    .ThenBy(t => t.FechaEntrega).ToList();


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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Finalizar(int TareaId, int ProyectoId)
        {
            var tarea = await _contenedorTrabajo.Tarea.GetByIdAsync(TareaId);

            if (tarea == null) return RedirectToAction("Index", new { id = ProyectoId });

            if(await _contenedorTrabajo.Reporte.GetFirstOrDefaultAsync(r => r.TareaId == TareaId
                && r.Estado == EstadosReporte.PendienteRevisar) != null)
            {
                return RedirectToAction("Index", new { id = ProyectoId });
            }

            tarea.Estado = EstadosTarea.Finalizada;
            tarea.FechaTermino = DateTime.Now;

            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index", new { id = ProyectoId });
        }
    }
}
