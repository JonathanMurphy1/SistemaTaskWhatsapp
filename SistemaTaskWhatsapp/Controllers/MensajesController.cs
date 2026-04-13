using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
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
                ListaEmpresas = await _contenedorTrabajo.Empresa.GetEmpresasDropdown(),
                ListaFestivos = await _contenedorTrabajo.MensajeDiaFestivo.ObtenerDiasFestivos()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MensajeVM model)
        {

            if (!ModelState.IsValid)
            {
                model.ListaEmpresas = await _contenedorTrabajo.Empresa.GetEmpresasDropdown();
                model.ListaFestivos = await _contenedorTrabajo.MensajeDiaFestivo.ObtenerDiasFestivos();
                return View(model);
            }

            //Crear nuevo festivo si el usuario llenó campos
            if (model.NuevosFestivos != null && model.NuevosFestivos.Any())
            {
                foreach (var festivo in model.NuevosFestivos)
                {
                    //Validación básica
                    if (festivo.Date == default || string.IsNullOrWhiteSpace(festivo.LocalName))
                        continue;

                    //Evitar duplicados por fecha
                    var existente = await _contenedorTrabajo.DiaFestivo
                        .GetFirstOrDefaultAsync(x => x.Date == festivo.Date);

                    if (existente != null)
                    {
                        //Si existe usar ese
                        model.FestivosSeleccionados.Add(existente.Id);
                    }
                    else
                    {
                        await _contenedorTrabajo.DiaFestivo.AddAsync(festivo);
                        await _contenedorTrabajo.SaveAsync(); // necesario para obtener ID

                        model.FestivosSeleccionados.Add(festivo.Id);
                    }
                }
            }

            model.Mensaje.FechaCreacion = DateTime.Now;
            model.Mensaje.Activo = false;

            await _contenedorTrabajo.Mensaje.AddAsync(model.Mensaje);
            await _contenedorTrabajo.SaveAsync();

            //Relación mensaje - festivos
            if (model.FestivosSeleccionados != null && model.FestivosSeleccionados.Any())
            {
                foreach (var festivoId in model.FestivosSeleccionados.Distinct())
                {
                    await _contenedorTrabajo.MensajeDiaFestivo.AddAsync(new MensajeDiaFestivo
                    {
                        MensajeId = model.Mensaje.Id,
                        DiaFestivoId = festivoId
                    });
                }

                await _contenedorTrabajo.SaveAsync();
            }

            return RedirectToAction("Index");
        }

        //Función para eliminar los días festivo del mensaje
        [HttpPost]
        public async Task<IActionResult> EliminarFestivo(int mensajeId, int festivoId)
        {
            if (mensajeId <= 0 || festivoId <= 0)
                return BadRequest();

            var relacion = await _contenedorTrabajo.MensajeDiaFestivo
                .GetFirstOrDefaultAsync(x =>
                    x.MensajeId == mensajeId &&
                    x.DiaFestivoId == festivoId);

            if (relacion == null)
                return NotFound();

            _contenedorTrabajo.MensajeDiaFestivo.Remove(relacion);
            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Edit", new { id = mensajeId });
        }

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
                // Registrar
                RecurringJob.AddOrUpdate<MensajesService>(
                    $"mensaje-{mensaje.Id}",
                    x => x.EnviarMensajeProgramado(mensaje.Id),
                    mensaje.Cron,
                   TimeZoneInfo.Local
                );
            }
            else
            {
                // eliminar de Hangfire
                RecurringJob.RemoveIfExists($"mensaje-{mensaje.Id}");
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var mensaje = await _contenedorTrabajo.Mensaje.GetByIdAsync(id);

            if (mensaje == null)
            {
                return RedirectToAction("Index");
            }

            var festivosRelacionados = await _contenedorTrabajo.MensajeDiaFestivo
                                              .GetAllAsync(x => x.MensajeId == id);

            var model = new MensajeVM
            {
                Mensaje = mensaje,
                ListaEmpresas = await _contenedorTrabajo.Empresa.GetEmpresasDropdown(),
                ListaFestivos = await _contenedorTrabajo.MensajeDiaFestivo.ObtenerDiasFestivos(),
                FestivosSeleccionados = festivosRelacionados.Select(x => x.DiaFestivoId).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(MensajeVM model)
        {
            if (!ModelState.IsValid)
            {
                model.ListaEmpresas = await _contenedorTrabajo.Empresa.GetEmpresasDropdown();
                return View(model);
            }


            //Eliminar relaciones actuales
            var relaciones = await _contenedorTrabajo.MensajeDiaFestivo
                .GetAllAsync(x => x.MensajeId == model.Mensaje.Id);

            foreach (var r in relaciones)
            {
                _contenedorTrabajo.MensajeDiaFestivo.Remove(r);
            }

            //Agregar nuevas
            foreach (var festivoId in model.FestivosSeleccionados)
            {
                await _contenedorTrabajo.MensajeDiaFestivo.AddAsync(new MensajeDiaFestivo
                {
                    MensajeId = model.Mensaje.Id,
                    DiaFestivoId = festivoId
                });
            }

            _contenedorTrabajo.Mensaje.Update(model.Mensaje);
            await _contenedorTrabajo.SaveAsync();

            //Editar el mensaje en hangfire
            if (model.Mensaje.Activo)
            {
                RecurringJob.AddOrUpdate<MensajesService>(
                    $"mensaje-{model.Mensaje.Id}",
                    x => x.EnviarMensajeProgramado(model.Mensaje.Id),
                    model.Mensaje.Cron,
                    TimeZoneInfo.Local
                );
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var mensaje = await _contenedorTrabajo.Mensaje.GetByIdAsync(id);

            if (mensaje == null) return RedirectToAction("Index");

            RecurringJob.RemoveIfExists($"mensaje-{mensaje.Id}");

            _contenedorTrabajo.Mensaje.Remove(mensaje);
            await _contenedorTrabajo.SaveAsync();
            return RedirectToAction("Index");

        }

    }
}