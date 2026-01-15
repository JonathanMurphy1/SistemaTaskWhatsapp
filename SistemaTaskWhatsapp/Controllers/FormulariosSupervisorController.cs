using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Models.ViewModels;
using SistemaTaskWhatsapp.Utilidades;
using Twilio.Jwt.AccessToken;
using Twilio.TwiML.Messaging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

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
        public async Task<IActionResult> FormularioEditarTarea(int id, string token)
        {
            var validacion = await ValidarToken(token);
            if (validacion != null)
                return validacion;

            var tarea = await _contenedorTrabajo.Tarea.GetFirstOrDefaultAsync(t => t.Id == id && t.Estado == EstadosTarea.Pendiente);

            if(tarea == null)
                return RedirectToAction("Resultado", new {mensaje = "Error al buscar la tarea", resultado = false});

            var vm = new EditarTareaFormSupervisorVM
            {
                Tarea = tarea,
                Token = token
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FormularioEditarTarea(EditarTareaFormSupervisorVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var validacion = await ValidarToken(model.Token);
            if (validacion != null)
                return validacion;

            string mensaje = "";
            bool resultado = false;

            var tareaBd = await _contenedorTrabajo.Tarea.GetFirstOrDefaultAsync(t => t.Id == model.Tarea.Id && t.Estado == EstadosTarea.Pendiente);

            if (tareaBd == null)
            {
                mensaje = "No se encontro la tarea en la base de datos";
                return RedirectToAction("Resultado", new { mensaje = mensaje, resultado = resultado });
            }

            if (model.Tarea.FechaEntrega < DateTime.Now)
            {
                ModelState.AddModelError("Fechaentrega", "La fecha de entrega debe ser mayor a la actual");
                return View(model);
            }

            tareaBd.Nombre = model.Tarea.Nombre;
            tareaBd.Descripcion = model.Tarea.Descripcion;
            tareaBd.FechaEntrega = model.Tarea.FechaEntrega;

            _contenedorTrabajo.Tarea.Update(tareaBd);
            await _contenedorTrabajo.SaveAsync();

            mensaje = "Tarea editada correctamente";
            resultado = true;

            return RedirectToAction("Resultado", new {mensaje = mensaje, resultado = resultado });
        }

        //Funciones
        private async Task<IActionResult?> ValidarToken(string token)
        {
            if (token == null)
                return RedirectToAction("Resultado", new { mensaje = "No se envio el token", resultado = false });

            var sesion = await _contenedorTrabajo.ChatSession
                .GetFirstOrDefaultAsync(c => c.TokenFormularios == token);

            if (sesion == null)
                return RedirectToAction("Resultado", new { mensaje = "Token no valido", resultado = false });

            if (sesion.FechaCreacionToken == null || sesion.FechaCreacionToken.Value.AddMinutes(15) < DateTime.Now)
                return RedirectToAction("Resultado", new { mensaje = "Token caducado", resultado = false });

            return null;
        }
    }
}
