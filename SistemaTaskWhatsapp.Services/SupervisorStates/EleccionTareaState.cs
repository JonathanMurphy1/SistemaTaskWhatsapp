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
    internal class EleccionTareaState : ISupervisorState
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public EleccionTareaState(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        public async Task<string> HandleAsync(Usuario usuario, ChatSession sesion, string mensaje)
        {
            if (!int.TryParse(mensaje, out int tareaId))
            {
                return "Ingrese un valor valido"; // o maneja el error
            }

            var tarea = await _contenedorTrabajo.Tarea
                .GetFirstOrDefaultAsync(t => t.Id == tareaId && t.Estado == EstadosTarea.Pendiente);

            if (tarea == null)
            {
                return "No se encontro la tarea";
            }


            sesion.TokenFormularios = Guid.NewGuid().ToString();
            sesion.FechaCreacionToken = DateTime.Now;
            sesion.EstadoStep = "Inicio";
            return $"Ingrese al siguente link para registrar colaboradores a una tarea: https://4cmlk6kl-7045.usw3.devtunnels.ms/FormulariosSupervisor/AsignarColaboradores?id={tarea.Id}&token={sesion.TokenFormularios}";
        }
    }
}
