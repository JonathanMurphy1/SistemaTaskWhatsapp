using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Data;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Models.ViewModels;
using SistemaTaskWhatsapp.Utilidades;
using SistemaTaskWhatsapp.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Services
{
    public class EmpleadoFlowService : IEmpleadoFlowService
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public EmpleadoFlowService(IContenedorTrabajo contenedorTrabajo) 
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        public async Task<string> ProcesarAsync(
           Usuario usuario,
           ChatSession sesion,
           string mensaje)
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
                                r => !r.VistoEmpleado && r.Reporte.Tarea.TareaEmpleados.Any(te => te.EmpleadoId == empleado.Id));

                    respuesta = "Buen dia {usuario.Nombre}\n" +
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
                        //Mostrar tareas////////////////
                        case "1":
                            var listaTarea = await _contenedorTrabajo.Tarea.GetAllAsync(
                                  t => t.Estado == EstadosTarea.Pendiente && t.TareaEmpleados.Any(te => te.EmpleadoId == empleado.Id), includeProperties: "Proyecto");

                            if (listaTarea == null || !listaTarea.Any())
                            {
                                mensaje = "No hay tareas en este momento, Felicidades." +
                                                "\nEscriba cualquier cosa para volver al menú principal.";
                                break;
                            }
                            else
                            {
                                respuesta = "Mis tareas pendientes\n" +
                                    "_______________________________________________________\n";

                                foreach (var item in listaTarea)
                                {
                                    respuesta += $"Nombre: {item.Nombre}\n" +
                                        $"Tarea con ID: {item.Id}\n" +
                                        $"Proyecto: {item.Proyecto?.Nombre}\n" +
                                        $"Descripción: {item.Descripcion}\n" +
                                        $"Fecha de inicio: {item.FechaInicio:dd/MM/yyyy}\n" +
                                        $"Fecha de entrega: {item.FechaEntrega:dd/MM/yyyy}\n" +
                                        $"_______________________________________________________\n";
                                }
                            }
                            respuesta += "\nEscriba cualquier cosa para volver al menú principal.";
                            sesion.EstadoStep = "Inicio";
                            break;

                        //Enviar reporte/////////////////////////////////////////////
                        case "2":
                            respuesta += "Ingresa el Id de la tarea para hacer su reporte.\n" +
                                        "Escribe *Inicio* para volver al menú principal";

                            sesion.EstadoStep = "ValidarTarea";
                            break;

                        //Ver retroalimentaciones///////////////////////////////////
                        case "3":
                            var listaRetroalimentacion = await _contenedorTrabajo.Retroalimentacion.GetAllAsync(
                                                  r => !r.VistoEmpleado && r.Reporte.Tarea.TareaEmpleados.Any(te => te.EmpleadoId == empleado.Id),
                                                  includeProperties: "Reporte,Supervisor"
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
                            await _contenedorTrabajo.SaveAsync();
                            break;

                        default:
                            return "Opción no valida";
                    }
                    break;
                //Validar tarea
                case "ValidarTarea":
                    sesion.DatosParciales = "";

                    var tarea = await _contenedorTrabajo.Tarea
                        .GetFirstOrDefaultAsync(t => t.Id.ToString() == mensaje && t.Estado == EstadosTarea.Pendiente);

                    if (tarea == null)
                    {
                        respuesta = "La tarea seleccionada no se encontró o ya se finalizó";
                        break;
                    }

                    respuesta =
                        $"Tarea encontrada\n" +
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

                //Guardar nombre y solicitar el resumen de actividades
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

                //Guardar contentindo y solicitar los incovenientes durante las actividades
                case "ComentarioReporte":
                    reporte = SessionJsonHelper.GetData<Reporte>(sesion.DatosParciales);
                    reporte.ComentarioEmpleado = mensaje;

                    sesion.DatosParciales = SessionJsonHelper.SetData(reporte);

                    sesion.EstadoStep = "GuardarReporte";
                    break;
                // Guardar Reporte
                case "GuardarReporte":
                    reporte = SessionJsonHelper.GetData<Reporte>(sesion.DatosParciales);

                    reporte.Estado = EstadosReporte.PendienteRevisar;

                    await _contenedorTrabajo.Reporte.AddAsync(reporte);
                    await _contenedorTrabajo.SaveAsync();

                    respuesta = "Reporte enviado correctamente. Ahora envíe sus evidencias.";
                    // sesion.EstadoStep = "GuardarEvidencia";
                    sesion.EstadoStep = "Inicio";

                    //Se guarda el reporte en la base de datos
                    sesion.DatosParciales = SessionJsonHelper.SetData(reporte);
                    break;


            }
            await _contenedorTrabajo.SaveAsync();
            return respuesta;
        }
    }
}
