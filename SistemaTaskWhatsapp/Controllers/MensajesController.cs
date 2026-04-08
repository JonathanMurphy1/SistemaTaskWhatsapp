using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Models.ViewModels;
using SistemaTaskWhatsapp.Services;
using Twilio.TwiML.Messaging;

namespace SistemaTaskWhatsapp.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class MensajesController : Controller
    {

        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly MensajesService _mensajesService;

        public MensajesController(IContenedorTrabajo contenedorTrabajo, MensajesService mensajesService)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _mensajesService = mensajesService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var listaMensajes = await _contenedorTrabajo.Mensaje.GetAllAsync(includeProperties: "Empresa");

            return View(listaMensajes);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new MensajeVM
            {

                ListaEmpresas = await _contenedorTrabajo.Empresa.GetEmpresasDropdown()
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Create(MensajeVM model)
        {

            if (!ModelState.IsValid)
            {
                model.ListaEmpresas = await _contenedorTrabajo.Empresa.GetEmpresasDropdown();
                return View(model);
            }

            model.Mensaje.FechaCreacion = DateTime.Now;
            model.Mensaje.Activo = false; 

            await _contenedorTrabajo.Mensaje.AddAsync(model.Mensaje);
            await _contenedorTrabajo.SaveAsync();


            return RedirectToAction("Index");
        }

        // Crear job en Hangfire
        //RecurringJob.AddOrUpdate<MensajesService>(
        //    $"mensaje-{model.Id}",
        //    x => x.EnviarMensajeProgramado(model.Id),
        //    model.Cron,
        //    TimeZoneInfo.Local
        //);


        //Funcion para Activar / Desactivar
        [HttpPost]
        public async Task<IActionResult> Toggle(int id)
        {
            var mensaje = await _contenedorTrabajo.Mensaje.GetByIdAsync(id);

            if (mensaje == null)
                return NotFound();

            mensaje.Activo = !mensaje.Activo;

            _contenedorTrabajo.Mensaje.Update(mensaje);
            await _contenedorTrabajo.SaveAsync();

            if (mensaje.Activo)
            {
                // volver a registrar
                //RecurringJob.AddOrUpdate<MensajesService>(
                //    $"mensaje-{mensaje.Id}",
                //    x => x.EnviarMensajeProgramado(mensaje.Id),
                //    mensaje.Cron,
                //    TimeZoneInfo.Local
                //);
            }
            else
            {
                // eliminar de Hangfire
                RecurringJob.RemoveIfExists($"mensaje-{mensaje.Id}");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var mensaje = await _contenedorTrabajo.Mensaje.GetByIdAsync(id);

            if (mensaje == null) return RedirectToAction("Index");

            //RecurringJob.RemoveIfExists($"mensaje-{mensaje.Id}");

            _contenedorTrabajo.Mensaje.Remove(mensaje);
            await _contenedorTrabajo.SaveAsync();
            return RedirectToAction("Index");

        }

    }
}