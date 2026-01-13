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
                    respuesta = "Elejiste la opción 4";
                    break;
                //Ver tareas pendientes
                case "5":
                    respuesta = "Elejiste la opción 5";
                    break;
                //Crear nueva tarea
                case "6":
                    respuesta = "Elejiste la opción 6";
                    break;
                //Mostrar colaboradores (esta opcion mostrara datos de los colaboradores y si tienen tareas asignadas)
                case "7":
                    respuesta = "Elejiste la opción 7";
                    break;
                default:
                    respuesta = "Opción no valida";
                    break;
            }
            return respuesta;
        }
    }
}
