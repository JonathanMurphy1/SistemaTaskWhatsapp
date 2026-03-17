using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Models.ViewModels;
using SistemaTaskWhatsapp.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Services.SupervisorStates
{
    internal class MenuTareaSeleccionState : ISupervisorState
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public MenuTareaSeleccionState(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        public async Task<string> HandleAsync(Usuario usuario, ChatSession sesion, string mensaje)
        {
            string respuesta = "";
            if (!int.TryParse(sesion.DatosParciales, out int tareaId))
            {
                sesion.EstadoStep = "Inicio";
                return "Error al obtener el Id"; // o maneja el error
            }

            var tarea = await _contenedorTrabajo.Tarea
                .GetFirstOrDefaultAsync(t => t.Id == tareaId && t.Estado == EstadosTarea.Pendiente);

            if (tarea == null)
            {
                sesion.EstadoStep = "Inicio";
                return "No se encontro la tarea";
            }

            switch (mensaje)
            {
                //Maracar una tarea como finalizada
                case "1":
                    respuesta = "¿Estas seguro de querer marcar esta tarea como completada?\n" +
                        "*Si*.Para marcar como completada\n" +
                        "Cualquier cosa para elejir otra acción";
                    sesion.EstadoStep = "MarcarTareaCompletada";
                    break;
                //Editar una tarea
                case "2":
                    string token = Guid.NewGuid().ToString();

                    sesion.TokenFormularios = token;
                    sesion.FechaCreacionToken = DateTime.Now; 

                    respuesta = $"Ingrese a este link para editar la tarea: https://kmtzj8db-5010.usw3.devtunnels.ms/FormulariosSupervisor/FormularioEditarTarea?id={tarea.Id}&token={token}\n" +

                    //respuesta = $"Ingrese a este link para editar la tarea: https://4cmlk6kl-7045.usw3.devtunnels.ms/FormulariosSupervisor/FormularioEditarTarea?id={tarea.Id}&token={token}\n" +
                        $"Puede elegir otra accion o escribir *Inicio* para volver al menú principal";
                    break;
                //Ver colaboradores de la tarea
                case "3":
                    var colaboradores = await _contenedorTrabajo.TareaEmpleado
                        .GetAllAsync(te => te.TareaId == tarea.Id && te.Empleado.Estado == EstadosEmpleado.Activo, includeProperties: "Empleado");

                    if (!colaboradores.Any())
                    {
                        respuesta = "Esta tarea no tiene colaboradores\n" +
                            "Elija otra acción";
                        break;
                    }

                    foreach (var item in colaboradores)
                    {
                        respuesta += $"Id: {item.EmpleadoId}\n" +
                            $"Nombre: {item.Empleado.Nombre}\n" +
                            $"---------------------------------------------\n";
                    }

                    respuesta += "Escribe otra accion que quieras realizar. Escribe *Inicio* para volver al menú principal";
                    break;
                default:
                    respuesta += "Escribe una acción valida. Escribe *Inicio* para volver al menú principal";
                    break;
            }

            return respuesta;
        }
    }
}
