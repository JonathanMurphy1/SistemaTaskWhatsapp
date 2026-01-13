using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaTaskWhatsapp.Utilidades;
using SistemaTaskWhatsapp.Models.ViewModels;

namespace SistemaTaskWhatsapp.Services
{
    public class SupervisorFlowService : ISupervisorFlowService
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public SupervisorFlowService(IContenedorTrabajo contenedorTrabajo)
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
            var supervisor = await _contenedorTrabajo.Supervisor.GetFirstOrDefaultAsync(e => e.UsuarioId == usuario.Id);

            if (supervisor == null) return "No se encontro el supervisor";

            if (supervisor.Estado == EstadosSupervisor.Inactivo)
                return "Tu cuenta de supervisor esta inactiva";

            if (mensajeLower == "inicio" || sesion.FechaActualizacion.AddMinutes(15) <= DateTime.Now)
            {
                sesion.DatosParciales = "";
                sesion.EstadoStep = "Inicio";
            }

            switch (sesion.EstadoStep)
            {
                //Inicio del flujo
                case "Inicio":
                    sesion.EstadoStep = "Menu";

                    var reportesPendientes = await _contenedorTrabajo.Reporte.GetAllAsync(r => r.Estado == EstadosReporte.PendienteRevisar);

                    respuesta = "Hola Supervisor\n" +
                        "---------------------------------------------\n" +
                        "Elija una opción\n" +
                        "*1*.Ver reportes sin revisar\n" +
                        "*2*.Crear proyecto\n" +
                        "*3*.Asignar tarea\n" +
                        "*4*.Tareas pendientes\n" +
                        "*5*.Tareas pendientes\n" +
                        "*6*.Crear tarea\n" +
                        "*7*.Mostrar colaboradores\n" +
                        $"Hay *{reportesPendientes.Count()}* reportes sin revisar";
                    break;
                //Menu principal
                case "Menu":
                    sesion.EstadoStep = "Inicio";

                    switch (mensaje)
                    {
                        //Mostrar lista de los reportes pendientes
                        case "1":
                            var reportes = await _contenedorTrabajo.Reporte
                                .GetAllAsync(r => r.Estado == EstadosReporte.PendienteRevisar, includeProperties: "Empleado,Tarea.Proyecto");

                            if(!reportes.Any())
                            {
                                mensaje = "No tienes reportes por revisar";
                            }

                            foreach (var item in reportes)
                            {
                                respuesta += $"Id: {item.Id}\n" +
                                    $"Titulo: {item.Nombre}\n" +
                                    $"Colaborador: {item.Empleado.Nombre}\n" +
                                    $"Tarea: {item.Tarea.Nombre}\n" +
                                    $"Proyecto: {item.Tarea.Proyecto.Nombre}\n" +
                                    $"-----------------------------------------------\n";
                            }

                            respuesta += "Ingresa el Id del reporte para ver detalles.\n" +
                                "Escribe *Inicio* para volver al menú principal";

                            sesion.EstadoStep = "DetalleReporte";
                            break;
                        //Crear proyecto
                        case "2":
                            respuesta = "Entre al siguiente link para registrar el proyecto: https://4cmlk6kl-7045.usw3.devtunnels.ms/Proyectos/Create";
                            sesion.EstadoStep = "Inicio";
                            break;
                        //Asignar tarea
                        case "3":
                            respuesta = "Elejiste la opción 3";
                            break;
                        case "4":
                            respuesta = "Elejiste la opción 4";
                            break;
                        case "5":
                            respuesta = "Elejiste la opción 5";
                            break;
                        case "6":
                            respuesta = "Elejiste la opción 6";
                            break;
                        case "7":
                            respuesta = "Elejiste la opción 7";
                            break;
                        default:
                            respuesta = "Opción no valida";
                            break;
                    }
                    break;
                //Mostrar los detalles del reporte
                case "DetalleReporte":
                    sesion.DatosParciales = "";
                    if(mensajeLower == "inicio")
                    {
                        respuesta = "Volviendo al inicio";
                        break;
                    }

                    if (!int.TryParse(mensaje, out int reporteId))
                    {

                        respuesta = "Ingrese un valor valido"; // o maneja el error
                        break;
                    }

                    var reporte = await _contenedorTrabajo.Reporte
                        .GetFirstOrDefaultAsync(r => r.Id == reporteId && r.Estado == EstadosReporte.PendienteRevisar
                        , includeProperties:"Empleado,Tarea.Proyecto");

                    if (reporte == null)
                    {
                        respuesta = "No se encontro el reporte, elija uno valido";
                        break;
                    }

                    respuesta = $"Id: {reporte.Id}\n" +
                        $"Titulo: {reporte.Nombre}\n" +
                        $"Tarea: {reporte.Tarea.Nombre}\n" +
                        $"Proyecto: {reporte.Tarea.Proyecto.Nombre}\n" +
                        $"Colaborador: {reporte.Empleado.Nombre}\n" +
                        $"Contenido: {reporte.Contenido}\n" +
                        $"Inconvenientes: {reporte.Inconvenientes ?? "Sin inconvenientes"}\n" +
                        $"Comentarios: {reporte.ComentarioEmpleado ?? "Sin comentarios"}\n" +
                        $"Fecha de subida: {reporte.FechaSubida.ToString("dd/MM/yyyy")}\n" +
                        $"-------------------------------------------------------------------\n" +
                        $"¿Quieres dar retroalimentación de esta tarea?\n" +
                        $"Escribe *Si* si quieres hacerlo, o cualquier cosa para revisar otros reportes";

                    sesion.EstadoStep = "DarRetroalimentacion";
                    var data = SessionJsonHelper.GetData<RevisarVM>(sesion.DatosParciales);
                    data.TareaId = reporte.TareaId;
                    data.ReporteId = reporte.Id;
                    sesion.DatosParciales = SessionJsonHelper.SetData(data);
                    break;
                //Dar la retroalimentacion
                case "DarRetroalimentacion":
                    if(mensajeLower != "si")
                    {
                        sesion.EstadoStep = "DetalleReporte";
                        respuesta = "Escriba el Id de otro reporte que quiera revisar.\n" +
                            "Escriba *Inicio* para volver al menú principal";
                        break;
                    }

                    sesion.EstadoStep = "AceptarReporte";
                    respuesta = "Escriba sus comentarios acerca de los avances y el reporte";
                    break;
                //Aceptar o rechazar el reporte
                case "AceptarReporte":
                    data = SessionJsonHelper.GetData<RevisarVM>(sesion.DatosParciales);
                    data.Comentario = mensaje;
                    
                    sesion.DatosParciales = SessionJsonHelper.SetData(data);

                    respuesta = "¿Deseas aceptar este reporte?\n" +
                        "*Si*.Aceptar\n" +
                        "*No*.Rechazar";

                    sesion.EstadoStep = "GuardarRetroalimentacion";
                    
                    break;
                //Guardar Retroalimentacion
                case "GuardarRetroalimentacion":
                    data = SessionJsonHelper.GetData<RevisarVM>(sesion.DatosParciales);

                    if (mensajeLower == "si") data.EstadoReporte = EstadosReporte.Aceptado;
                    else if (mensajeLower == "no") data.EstadoReporte = EstadosReporte.Rechazado;
                    else
                    {
                        respuesta = "Opción no valida, elija una de las dos o escriba *Inicio* para volver al menú de inicio";
                        break;
                    }

                    var nuevaRetroalimentacion = new Retroalimentacion
                    {
                        Comentario = data.Comentario,
                        Fecha = DateTime.Now,
                        SupervisorId = supervisor.Id,
                        ReporteId = data.ReporteId,
                    };

                    await _contenedorTrabajo.Retroalimentacion.AddAsync(nuevaRetroalimentacion);

                    reporte = await _contenedorTrabajo.Reporte.GetByIdAsync(data.ReporteId);
                    reporte.Estado = data.EstadoReporte;
                    respuesta = "Retroalimentación enviada correctamente, volviendo al inicio";
                    sesion.EstadoStep = "Inicio";
                    sesion.DatosParciales = "";
                    _contenedorTrabajo.Reporte.Update(reporte);
                        break;
                //Si el paso de la sesion no es valido
                default:
                    respuesta = "Opción no valida";
                    break;
            }
            sesion.FechaActualizacion = DateTime.Now;
            _contenedorTrabajo.ChatSession.Update(sesion);
            await _contenedorTrabajo.SaveAsync();
            return respuesta;
        }
    }
}
