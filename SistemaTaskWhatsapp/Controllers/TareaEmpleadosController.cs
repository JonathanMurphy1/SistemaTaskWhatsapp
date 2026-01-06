using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Models.ViewModels;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Controllers
{
    public class TareaEmpleadosController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public TareaEmpleadosController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }


        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            var tarea = await _contenedorTrabajo.Tarea.
                GetFirstOrDefaultAsync(x => x.Id == id, includeProperties: "TareaEmpleados.Empleado.Usuario");

            var model = new ColaboradoresVM
            {
                Tarea = tarea,
                ListaEmpleados = await _contenedorTrabajo.Empleado.ObtenerListaEmpleados()
            };

            ViewBag.ProyectoId = tarea.ProyectoId;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? tareaId, int? colaboradorId)
        {
            if(tareaId == null || colaboradorId == null)
            {
                return RedirectToAction ("Index", new {id = tareaId });
            }

            if(await _contenedorTrabajo.TareaEmpleado.
                GetFirstOrDefaultAsync(te => te.EmpleadoId == colaboradorId && te.TareaId == tareaId) != null)
            {
                return RedirectToAction("Index", new { id = tareaId });
            }

            var nuevoColaborador = new TareaEmpleado
            {
                TareaId = (int)tareaId,
                EmpleadoId = (int)colaboradorId
            };

            await _contenedorTrabajo.TareaEmpleado.AddAsync(nuevoColaborador);
            await _contenedorTrabajo.SaveAsync();
            return RedirectToAction("Index", new { id = tareaId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? tareaEmpleadoId, int? tareaId)
        {
            if (tareaId == null || tareaEmpleadoId == null)
            {
                return RedirectToAction("Index", new { id = tareaId });
            }

            await _contenedorTrabajo.TareaEmpleado.RemoveByIdAsync((int)tareaEmpleadoId);
            await _contenedorTrabajo.SaveAsync();
            return RedirectToAction("Index", new { id = tareaId });
        }
    }
}
