using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Models.ViewModels;
using SistemaTaskWhatsapp.Utilidades;
using Twilio.TwiML.Messaging;

namespace SistemaTaskWhatsapp.Controllers
{
    public class FormulariosSupervisorController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public FormulariosSupervisorController(IContenedorTrabajo contenedorTrabajo)
        {
                _contenedorTrabajo = contenedorTrabajo;
        }

        [HttpGet]
        public IActionResult Resultado(string mensaje, bool resultado)
        {
            var model = new ResultadoFormSupervisorVM
            {
                Mensaje = mensaje,
                Resultado = resultado
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> FormularioEditarTarea(int id)
        {
            var tarea = await _contenedorTrabajo.Tarea.GetFirstOrDefaultAsync(t => t.Id == id && t.Estado == EstadosTarea.Pendiente);

            if(tarea == null)
            {
                return RedirectToAction("Resultado", new {mensaje = "Error al buscar la tarea", resultado = false});
            }

            return View(tarea);
        }

        [HttpPost]
        public async Task<IActionResult> FormularioEditarTarea(Tarea model)
        {
            if (!ModelState.IsValid) return View(model);

            string mensaje = "";
            bool resultado = false;

            var tareaBd = await _contenedorTrabajo.Tarea.GetFirstOrDefaultAsync(t => t.Id == model.Id && t.Estado == EstadosTarea.Pendiente);

            if (tareaBd == null)
            {
                mensaje = "No se encontro la tarea en la base de datos";
                return RedirectToAction("Resultado", new { mensaje = mensaje, resultado = resultado });
            }

            if (model.FechaEntrega < DateTime.Now)
            {
                ModelState.AddModelError("Fechaentrega", "La fecha de entrega debe ser mayor a la actual");
                return View(model);
            }

            tareaBd.Nombre = model.Nombre;
            tareaBd.Descripcion = model.Descripcion;
            tareaBd.FechaEntrega = model.FechaEntrega;

            _contenedorTrabajo.Tarea.Update(tareaBd);
            await _contenedorTrabajo.SaveAsync();

            mensaje = "Tarea editada correctamente";
            resultado = true;

            return RedirectToAction("Resultado", new {mensaje = mensaje, resultado = resultado });
        }
    }
}
