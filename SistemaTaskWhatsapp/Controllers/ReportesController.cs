using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Models.ViewModels;
using SistemaTaskWhatsapp.Utilidades;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Controllers
{
    public class ReportesController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public ReportesController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var lista = await _contenedorTrabajo.Reporte.GetAllAsync(includeProperties: "Tarea,Empleado.Usuario");

            return View(lista);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new ReporteVM
            {
                ListaTareas = await _contenedorTrabajo.Tarea.ObtenerTareasVigentes(),
                ListaEmpleados = await _contenedorTrabajo.Empleado.ObtenerListaEmpleados()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ReporteVM model)
        {
            if (!ModelState.IsValid)
            {
                model.ListaTareas = await _contenedorTrabajo.Tarea.ObtenerTareasVigentes();
                model.ListaEmpleados = await _contenedorTrabajo.Empleado.ObtenerListaEmpleados();

                return View(model);
            }

            model.Reporte.FechaSubida = DateTime.Now;

            await _contenedorTrabajo.Reporte.AddAsync(model.Reporte);

            var colaboradorExistente = await _contenedorTrabajo.TareaEmpleado.GetFirstOrDefaultAsync
                (te => te.TareaId == model.Reporte.TareaId && te.EmpleadoId == model.Reporte.EmpleadoId);

            if(colaboradorExistente == null)
            {
                var nuevoColaborador = new TareaEmpleado
                {
                    EmpleadoId = model.Reporte.EmpleadoId,
                    TareaId = model.Reporte.TareaId
                };

                await _contenedorTrabajo.TareaEmpleado.AddAsync(nuevoColaborador);
            }
      
            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var reporte = await _contenedorTrabajo.Reporte.GetByIdAsync(id);

            if (reporte == null) return RedirectToAction("Index");

            var model = new ReporteVM
            {
                Reporte = reporte,
                ListaTareas = await _contenedorTrabajo.Tarea.ObtenerTareasVigentes(),
                ListaEmpleados = await _contenedorTrabajo.Empleado.ObtenerListaEmpleados()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ReporteVM model)
        {
            if (!ModelState.IsValid)
            {
                model.ListaTareas = await _contenedorTrabajo.Tarea.ObtenerTareasVigentes();
                model.ListaEmpleados = await _contenedorTrabajo.Empleado.ObtenerListaEmpleados();

                return View(model);
            }

            _contenedorTrabajo.Reporte.Update(model.Reporte);

            var colaboradorExistente = await _contenedorTrabajo.TareaEmpleado.GetFirstOrDefaultAsync
                (te => te.TareaId == model.Reporte.TareaId && te.EmpleadoId == model.Reporte.EmpleadoId);

            if (colaboradorExistente == null)
            {
                var nuevoColaborador = new TareaEmpleado
                {
                    EmpleadoId = model.Reporte.EmpleadoId,
                    TareaId = model.Reporte.TareaId
                };

                await _contenedorTrabajo.TareaEmpleado.AddAsync(nuevoColaborador);
            }

            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var reporte = await _contenedorTrabajo.Reporte.GetByIdAsync(id);

            if (reporte == null) return RedirectToAction("Index");

            _contenedorTrabajo.Reporte.Remove(reporte);
            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index");
        }
    }
}
