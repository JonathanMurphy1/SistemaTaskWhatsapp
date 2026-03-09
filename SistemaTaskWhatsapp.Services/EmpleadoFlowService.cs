using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Data;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Models.ViewModels;
using SistemaTaskWhatsapp.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SistemaTaskWhatsapp.Services
{
    public class EmpleadoFlowService : IEmpleadoFlowService
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IConfiguration _config;

        public EmpleadoFlowService(IContenedorTrabajo contenedorTrabajo, IConfiguration config)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _config = config;

        }
        public async Task<string> ProcesarAsync(
           Usuario usuario,
           ChatSession sesion,
           string mensaje
           )
        {
            string respuesta = "";
            string mensajeLower = mensaje.ToLower();

            var empleado = await _contenedorTrabajo.Empleado.GetFirstOrDefaultAsync(e => e.UsuarioId == usuario.Id);

            if (empleado == null) return "No se encontro el empleado";

            if (empleado.Estado == EstadosEmpleado.Inactivo)
                return "Tu cuenta de empleado está inactiva";

            if (mensajeLower == "inicio")
            {
                sesion.DatosParciales = "";
                sesion.EstadoStep = "Inicio";
            }

            switch (sesion.EstadoStep)
            {
                //Inicio del flujo
                case "Inicio":
                    sesion.EstadoStep = "Menu";

                    var tareasPendientes = await _contenedorTrabajo.Tarea.GetAllAsync(
                                t => t.Estado == EstadosTarea.Pendiente && t.TareaEmpleados.Any(te => te.EmpleadoId == empleado.Id));

                    var retroalimentacionesPendientes = await _contenedorTrabajo.Retroalimentacion.GetAllAsync(
                                r => !r.VistoEmpleado && r.Reporte.EmpleadoId == empleado.Id);

                    respuesta = $"Buen dia {usuario.Nombre}\n" +
                        "-----------------------------------------------------\n" +
                        "Elija una opción\n" +
                        "*1*.Consultar tareas\n" +
                        "*2*.Enviar reporte\n" +
                        "*3*.Revisar retroalimentaciones pendientes\n" +
                         "----------------------------------------------------\n" +
                        $"Hay *{tareasPendientes.Count()}* tareas pendientes\n" +
                        $"Hay *{retroalimentacionesPendientes.Count()}* retroalimentaciones sin revisar";
                    break;
                //Menu principal
                case "Menu":
                    sesion.EstadoStep = "Inicio";
                    await _contenedorTrabajo.SaveAsync();
                    switch (mensaje)
                    {
                        //Mostrar tareas/////////////////
                        case "1":
                        {
                                var listaTarea = (await _contenedorTrabajo.Tarea.GetAllAsync(
                                  t => t.Estado == EstadosTarea.Pendiente && t.TareaEmpleados.Any(te => te.EmpleadoId == empleado.Id),includeProperties: "Proyecto"
                                                    )) .OrderBy(t => t.FechaEntrega).ToList();

                                if (listaTarea == null || !listaTarea.Any())
                                {
                                    respuesta = "No hay tareas en este momento, Felicidades." +
                                                    "\nEscriba cualquier cosa para volver al menú principal.";
                                    break;
                                }
                                else
                                {
                                    respuesta = "Mis tareas pendientes\n" +
                                        "_______________________________________________________\n";

                                    foreach (var item in listaTarea)
                                    {
                                        var fechaEntrega = item.FechaEntrega.Value.Date;
                                        var hoy = DateTime.Now.Date;
                                        var diasRestantes = (fechaEntrega - hoy).Days;

                                        string estadoEntrega;
                                        if (diasRestantes < 0)
                                            estadoEntrega = $"Vencida hace {Math.Abs(diasRestantes)} días";
                                        else if (diasRestantes == 0)
                                            estadoEntrega = "Vence hoy";
                                        else if (diasRestantes == 1)
                                            estadoEntrega = "Vence mañana";
                                        else
                                            estadoEntrega = $"Quedan {diasRestantes} días para su entrega";

                                        respuesta += $"Nombre: {item.Nombre}\n" +
                                            $"Tarea con ID: {item.Id}\n" +
                                            $"Proyecto: {item.Proyecto?.Nombre}\n" +
                                            $"Descripción: {item.Descripcion}\n" +
                                            $"Fecha de inicio: {item.FechaInicio:dd/MM/yyyy}\n" +
                                            $"Fecha de entrega: {item.FechaEntrega:dd/MM/yyyy}\n" +
                                            $"{estadoEntrega}\n" +
                                            $"_______________________________________________________\n";
                                    }
                                }

                                respuesta += "\nEscriba cualquier cosa para volver al menú principal.";
                                sesion.EstadoStep = "Inicio";
                                break;
                        }
                        //Enviar reporte/////////////////////////////////////////////
                        case "2":
                        {   
                            var listaTarea = await _contenedorTrabajo.Tarea.GetAllAsync(
                                     t => t.Estado == EstadosTarea.Pendiente && t.TareaEmpleados.Any(
                                         te => te.EmpleadoId == empleado.Id), includeProperties: "Proyecto");

                            if (listaTarea == null || !listaTarea.Any())
                            {
                                respuesta = "No hay tareas en este momento para realizar un reporte." +
                                                   "\nEscriba cualquier cosa para volver al menú principal.";
                                break;
                            }
                            else
                            {
                                respuesta += "Ingresa el Id de la tarea para hacer su reporte.\n" +
                                         "Escribe *Inicio* para volver al menú principal";

                                sesion.EstadoStep = "ValidarTarea";
                                break;
                            }
                        }
                        //Ver retroalimentaciones///////////////////////////////////
                        case "3":
                            var listaRetroalimentacion = await _contenedorTrabajo.Retroalimentacion.GetAllAsync(
                                                r => !r.VistoEmpleado && r.Reporte.EmpleadoId == empleado.Id, includeProperties: "Reporte,Supervisor"
                            );

                            if (listaRetroalimentacion == null || !listaRetroalimentacion.Any())
                            {
                                respuesta = "No hay retroalimentaciones por ver en este momento." +
                                                    "\nEscriba cualquier cosa para volver al menú principal.";
                                break;
                            }
                            else
                            {
                                respuesta = "Mis retroalimentaciones\n" +
                                    "_______________________________________________________\n";

                                foreach (var item in listaRetroalimentacion)
                                {
                                    respuesta += $"Reporte: {item.Reporte?.Nombre}\n" +
                                        $"Hecho por: {item.Supervisor?.Nombre ?? "Supervisor no disponible"}\n" +
                                        $"Fecha: {item.Fecha:dd/MM/yyyy}\n" +
                                        $"Descripción: {item.Comentario}\n" +
                                        $"Estado: {item.Reporte?.Estado}\n" +
                                        $"_______________________________________________________\n";

                                    item.VistoEmpleado = true;
                                }

                            }

                            respuesta += "\nEscriba cualquier cosa para volver al menú principal.";
                            sesion.EstadoStep = "Inicio";
                            break;

                        default:
                            return "Opción no valida";
                    }
                    break;

                //Validar tarea
                case "ValidarTarea":
                    sesion.DatosParciales = ""; 

                    var tarea = await _contenedorTrabajo.Tarea.GetFirstOrDefaultAsync(
                                t => t.Id.ToString() == mensaje && t.Estado == EstadosTarea.Pendiente &&
                                    t.TareaEmpleados.Any(te => te.EmpleadoId == empleado.Id)
                    );

                    if (tarea == null)
                    {
                        respuesta = "La tarea seleccionada no se encontró o ya se finalizó." +
                                        "\nVuelva a escribir el ID o escriba *Inicio* para volver al menú principal.";
                        break;
                    }

                    respuesta =
                        $"Tarea encontrada con ID: {tarea.Id}\n" +
                        $"---------------------------------------------\n" +
                        $"Nombre: {tarea.Nombre}\n" +
                        $"Descripción: {tarea.Descripcion}\n" +
                        $"Fecha de entrega: {tarea.FechaEntrega:dd/MM/yyyy}\n" +
                        $"---------------------------------------------\n" +
                        $"Por favor escriba el nombre que tendrá su reporte";

                    sesion.EstadoStep = "NombreReporte";

                    var reporte = new Reporte
                    {
                        TareaId = tarea.Id,
                        EmpleadoId = empleado.Id,
                        FechaSubida = DateTime.Now
                    };

                    sesion.DatosParciales = SessionJsonHelper.SetData(reporte);
                    break;

                //Guardar nombre y solicitar el resumen da actividades
                case "NombreReporte":
                    reporte = SessionJsonHelper.GetData<Reporte>(sesion.DatosParciales);
                    reporte.Nombre = mensaje;

                    sesion.DatosParciales = SessionJsonHelper.SetData(reporte);

                    sesion.EstadoStep = "ContenidoReporte";
                    respuesta = "Por favor escriba un resumen de sus actividades";
                    break;

                //Guardar contenido y solicitar incovenientes
                case "ContenidoReporte":
                    reporte = SessionJsonHelper.GetData<Reporte>(sesion.DatosParciales);
                    reporte.Contenido = mensaje;

                    sesion.DatosParciales = SessionJsonHelper.SetData(reporte);

                    respuesta = "Escriba los inconvenientes que tuvo";
                    sesion.EstadoStep = "InconvenienteReporte";
                    break;

                //Guardar incovenientes y solicitar comentarios
                case "InconvenienteReporte":
                    reporte = SessionJsonHelper.GetData<Reporte>(sesion.DatosParciales);
                    reporte.Inconvenientes = mensaje;

                    sesion.DatosParciales = SessionJsonHelper.SetData(reporte);

                    respuesta = "Escriba sus comentarios respecto a la actividad realizada";
                    sesion.EstadoStep = "ComentarioReporte";
                    break;

                //Guardar comentario, guardar todo el reporte y solicitar las evidencias
                case "ComentarioReporte":
                    reporte = SessionJsonHelper.GetData<Reporte>(sesion.DatosParciales);
                    reporte.ComentarioEmpleado = mensaje;
                    reporte.Estado = EstadosReporte.PendienteRevisar;

                    await _contenedorTrabajo.Reporte.AddAsync(reporte);
                    await _contenedorTrabajo.SaveAsync();

                    respuesta = "Reporte enviado correctamente. Ahora envíe sus evidencias.";

                    //Capturar evidencia
                    sesion.DatosParciales = SessionJsonHelper.SetData(reporte.Id);
                    sesion.EstadoStep = "EnviarEvidencia";
                    break;

                //Repetir ciclo
                case "VolverEvidencia":
                    if (mensajeLower == "no")
                    {
                        respuesta = "Entendido. Escriba cualquier cosa para volver al menú principal.";
                        sesion.EstadoStep = "Inicio";
                        sesion.DatosParciales = "";
                        break;
                    }
                    else if (mensajeLower == "si")
                    {
                        respuesta = "Envíe otra evidencia por favor.";
                        sesion.EstadoStep = "EnviarEvidencia";
                        break;
                    }
                    else
                    {
                        respuesta = "Opción no válida.\n¿Quiere enviar otra evidencia? SI/NO";
                        sesion.EstadoStep = "VolverEvidencia";
                        break;
                    }

                //Guardar imagen y solicitar la descripción
                case "EnviarEvidencia":
                {
                    int reporteId = SessionJsonHelper.GetData<int>(sesion.DatosParciales);

                    if (!mensaje.StartsWith("https://"))
                    {
                        respuesta = "No se admiten mensajes de texto como evidencia.";
                        sesion.EstadoStep = "EnviarEvidencia";
                        break;
                    }

                    await GuardarArchivoDeWhatsApp(reporteId, mensaje);

                    respuesta = "Agregue una descripción para la evidencia enviada";
                    sesion.EstadoStep = "DescripcionEvidencia";
                    break;
                }

                //Guardar toda la evidencia y preguntar para volver a entrar al ciclo de nuevo
                case "DescripcionEvidencia":
                {
                    int reporteId = SessionJsonHelper.GetData<int>(sesion.DatosParciales);

                    var evidencias = await _contenedorTrabajo.Evidencia.GetAllAsync(e => e.ReporteId == reporteId);

                    var evidencia = evidencias.OrderByDescending(e => e.Id).FirstOrDefault();

                    evidencia.Descripcion = mensaje;
                    await _contenedorTrabajo.SaveAsync();

                    respuesta = "Evidencia guardada.\n¿Desea enviar otra evidencia? SI/NO";
                    sesion.EstadoStep = "VolverEvidencia";
                    break;
                }
            }
            await _contenedorTrabajo.SaveAsync();
            return respuesta;
        }

        //Metodos Auxiliares
        private async Task GuardarArchivoDeWhatsApp(int reporteId, string mediaUrl)
        {
            if (string.IsNullOrWhiteSpace(mediaUrl))
                return;

            string carpetaDestino = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "evidencias"
            );

            if (!Directory.Exists(carpetaDestino))
                Directory.CreateDirectory(carpetaDestino);

            using var http = new HttpClient();

            string token = _config["Twilio:BasicAuth"];

            http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", token);

            var response = await http.GetAsync(mediaUrl);

            if (!response.IsSuccessStatusCode)
                return;

            var bytes = await response.Content.ReadAsByteArrayAsync();

            // Detectar extensión según Content-Type
            string contentType = response.Content.Headers.ContentType?.MediaType ?? "";
            string extension = contentType switch
            {
                "image/jpeg" => ".jpg",
                "image/png"  => ".png",
                "application/pdf" => ".pdf",
                _ => ".bin"
            };

            string nombreArchivo = $"{Guid.NewGuid()}{extension}";
            string rutaCompleta = Path.Combine(carpetaDestino, nombreArchivo);

            await File.WriteAllBytesAsync(rutaCompleta, bytes);

            Evidencia evidencia = new Evidencia
            {
                ReporteId = reporteId,
                Url = "/uploads/evidencias/" + nombreArchivo,
                Descripcion = ""
            };

            await _contenedorTrabajo.Evidencia.AddAsync(evidencia);
            await _contenedorTrabajo.SaveAsync();
        }
    }
}
