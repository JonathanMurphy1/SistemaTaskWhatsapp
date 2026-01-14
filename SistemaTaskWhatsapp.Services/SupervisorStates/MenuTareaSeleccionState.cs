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
                case "1":
                    respuesta = "¿Estas seguro de querer marcar esta tarea como completada?\n" +
                        "*Si*.Para marcar como completada\n" +
                        "Cualquier cosa para elejir otra acción";
                    sesion.EstadoStep = "MarcarTareaCompletada";
                    break;
                case "2":
                    respuesta = "Aqui se vera proximamente un link con el formulario";
                    break;
                case "3":
                    respuesta = "Aqui se mostraran los colaboradores registrados en esta tarea";
                    break;
                default:
                    break;
            }

            return respuesta;
        }
    }
}
