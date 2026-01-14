using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Services.SupervisorStates
{
    internal class MenuState : ISupervisorState
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public MenuState(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }
        public async Task<string> HandleAsync(Usuario usuario, ChatSession sesion, string mensaje)
        {
            sesion.EstadoStep = "Inicio";
            string respuesta = "";
            switch (mensaje)
            {
                //Mostrar lista de los reportes pendientes
                case "1":
                    var reportes = await _contenedorTrabajo.Reporte
                        .GetAllAsync(r => r.Estado == EstadosReporte.PendienteRevisar, includeProperties: "Empleado,Tarea.Proyecto");

                    if (!reportes.Any())
                    {
                        return "No tienes reportes por revisar";
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
                    respuesta = "Elija una tarea\n" +
                        "----------------------------------\n";
                    var tareas = await _contenedorTrabajo.Tarea.GetAllAsync(t => t.Estado == EstadosTarea.Pendiente, includeProperties: "Proyecto.Empresa,TareaEmpleados");

                    foreach (var item in tareas)
                    {
                        int numColaboradores = item.TareaEmpleados.Count();
                        respuesta += $"Id: {item.Id}\n" +
                            $"Nombre: {item.Nombre}\n" +
                            $"Proyecto: {item.Proyecto.Nombre}\n" +
                            $"Empresa: {item.Proyecto.Empresa.Nombre}\n" +
                            $"Colaboradores: {(numColaboradores > 0 ? numColaboradores : "Sin colaboradores")}\n" +
                            $"---------------------------------------\n";
                    }
                    sesion.EstadoStep = "EleccionTarea";
                    break;
                //Ver proyectos pendientes
                case "4":
                    var proyectos = await _contenedorTrabajo.Proyecto.GetAllAsync(p => p.Estado == EstadosProyecto.Activo, includeProperties: "Empresa");
                    
                    if(!proyectos.Any())
                    {
                        respuesta = "No hay proyectos pendientes";
                        sesion.EstadoStep = "Inicio";
                        break;
                    }
                    
                    foreach (var item in proyectos)
                    {
                        var tareasPendientes = await _contenedorTrabajo.Tarea
                            .GetAllAsync(t => t.ProyectoId == item.Id && t.Estado == EstadosTarea.Pendiente);


                        respuesta += $"Id: {item.Id}\n" +
                            $"Nombre: {item.Nombre}\n" +
                            $"Descripción: {item.Descripcion}\n" +
                            $"Fecha de registro: {item.FechaRegistro.ToString("dd/MM/yyyy")}\n" +
                            $"Tareas pendientes: {tareasPendientes.Count()}\n" +
                            $"-------------------------------------------------\n";
                    }
                    respuesta += "Escriba cualquier cosa para volver al menú de inicio";
                    sesion.EstadoStep = "Inicio";
                    break;
                //Ver tareas pendientes
                case "5":
                    tareas = await _contenedorTrabajo.Tarea.GetAllAsync(t => t.Estado == EstadosTarea.Pendiente, includeProperties: "Proyecto,TareaEmpleados.Empleado");

                    if (!tareas.Any())
                    {
                        respuesta = "No hay tareas pendientes";
                        sesion.EstadoStep = "Inicio";
                        break;
                    }

                    foreach (var item in tareas)
                    {
                        respuesta += $"Id: {item.Id}\n" +
                            $"Nombre: {item.Nombre}\n" +
                            $"Descripción: {item.Descripcion}\n" +
                            $"Fecha de inicio: {item.FechaInicio.ToString("dd/MM/yyyy")}\n" +
                            $"Fecha de entrega: {item.FechaEntrega?.ToString("dd/MM/yyyy")}\n" +
                            $"Colaboradores: {item.TareaEmpleados.Count()}\n" +
                            $"-------------------------------------------------\n";
                    }

                    respuesta += "Escribe el Id de una tarea para realizar acciones\n" +
                        "Escriba *Inicio* para volver al menú principal";

                    sesion.EstadoStep = "MenuTarea";
                    break;
                //Crear nueva tarea
                case "6":
                    proyectos = await _contenedorTrabajo.Proyecto.GetAllAsync(p => p.Estado == EstadosProyecto.Activo, includeProperties: "Empresa");

                    if (!proyectos.Any())
                    {
                        respuesta = "No hay proyectos para agregar tareas";
                        sesion.EstadoStep = "Inicio";
                        break;
                    }

                    respuesta = "¿Para que proyecto quieres crear la nueva tarea?\n" +
                        "----------------------------------------------------------\n";

                    foreach (var item in proyectos)
                    {
                        respuesta += $"Id: {item.Id}\n" +
                            $"Nombre: {item.Nombre}\n" +
                            $"-------------------------------------------------\n";
                    }

                    respuesta += "Escriba el Id por favor";
                    sesion.EstadoStep = "RespuestaCrearTarea";
                    break;
                //Mostrar colaboradores (esta opcion mostrara datos de los colaboradores y si tienen tareas asignadas)
                case "7":
                    var empleados = await _contenedorTrabajo.Empleado.GetAllAsync(e => e.Estado == EstadosEmpleado.Activo, includeProperties: "Usuario");

                    var empleadosConTareas = new List<(Empleado empleado, int tareasPendientes)>();

                    foreach (var item in empleados)
                    {
                        var tareasPendientes = await _contenedorTrabajo.TareaEmpleado
                            .GetAllAsync(te =>
                                te.EmpleadoId == item.Id &&
                                te.Tarea.Estado == EstadosTarea.Pendiente,
                                includeProperties: "Tarea");

                        empleadosConTareas.Add((item, tareasPendientes.Count()));
                    }

                    empleadosConTareas = empleadosConTareas.OrderBy(e => e.tareasPendientes)
                        .ToList();

                    foreach (var item in empleadosConTareas)
                    {
                        respuesta += $"Id: {item.empleado.Id}\n" +
                            $"Nombre: {item.empleado.Nombre}\n" +
                            $"Fecha de registro: {item.empleado.FechaRegistro.ToString("dd/MM/yyyy")}\n" +
                            $"Telefono: {item.empleado.Usuario.Telefono}\n" +
                            $"Tareas pendientes: {item.tareasPendientes}\n" +
                            $"-------------------------------------------------------\n";
                    }
                    respuesta += "Escriba cualquier cosa para volver al menú de inicio";
                    sesion.EstadoStep = "Inicio";
                    break;
                default:
                    respuesta = "Opción no valida";
                    break;
            }
            return respuesta;
        }
    }
}
