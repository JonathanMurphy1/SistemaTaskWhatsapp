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
    internal class MarcarTareaCompletadaState : ISupervisorState
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public MarcarTareaCompletadaState(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        public async Task<string> HandleAsync(Usuario usuario, ChatSession sesion, string mensaje)
        {
            string respuesta = "";
            string mensajeLower = mensaje.ToLower();
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

            if(mensajeLower == "si")
            {
                tarea.Estado = EstadosTarea.Finalizada;
                _contenedorTrabajo.Tarea.Update(tarea);
                respuesta = "Tarea marcada como finalizada.\n" +
                    "Volviendo al menú de inicio";
                sesion.EstadoStep = "Inicio";
            }
            else
            {
                respuesta = "Elija otra accion del menú";
                sesion.EstadoStep = "MenuTareaSeleccion"; 
            }
                return respuesta;
        }
    }
}
