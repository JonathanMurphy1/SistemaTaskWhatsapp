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
    internal class MenuTareaState : ISupervisorState
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public MenuTareaState(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        public async Task<string> HandleAsync(Usuario usuario, ChatSession sesion, string mensaje)
        {
            int tareaId;
            string respuesta = "";
            
            if (!int.TryParse(mensaje, out tareaId))
            {
                return "Ingrese un valor valido"; // o maneja el error
            }

            var tarea = await _contenedorTrabajo.Tarea
                    .GetFirstOrDefaultAsync(t => t.Id == tareaId && t.Estado == EstadosTarea.Pendiente);

            if (tarea == null)
            {
                return "No se encontro la tarea";
            }

            respuesta = $"Tarea: {tarea.Nombre} (Id: {tarea.Id})\n" +
                $"--------------------------------------------------\n" +
                $"*1*.Marcar como completada\n" +
                $"*2*.Editar tarea\n" +
                $"*3*.Mostrar colaboradores\n" +
                $"-------------------------------------------------\n" +
                $"Elija una opción\n" +
                $"Escriba *Inicio* para volver al menú principal";
            sesion.EstadoStep = "MenuTareaSeleccion";
            sesion.DatosParciales = tarea.Id.ToString();
            return respuesta;
        }
    }
}
