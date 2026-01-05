using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
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
                GetFirstOrDefaultAsync(x => x.Id == id, includeProperties: "TareaEmpleados.Empleado");

            var model = new ColaboradoresVM
            {
                Tarea = tarea,
                ListaEmpleados = await _contenedorTrabajo.Empleado.ObtenerListaEmpleados()
            };

            ViewBag.ProyectoId = tarea.ProyectoId;

            return View(model);
        }
    }
}
