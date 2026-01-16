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
    internal class RespuestaCrearTareaState : ISupervisorState
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public RespuestaCrearTareaState(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }
        public async Task<string> HandleAsync(Usuario usuario, ChatSession sesion, string mensaje)
        {
            if (!int.TryParse(mensaje, out int proyectoId))
            {
                return "Ingrese un valor valido"; // o maneja el error
            }

            var proyecto = await _contenedorTrabajo.Proyecto
                .GetFirstOrDefaultAsync(t => t.Id == proyectoId && t.Estado == EstadosProyecto.Activo);

            if (proyecto == null)
            {
                return "No se encontro el proyecto";
            }

            sesion.TokenFormularios = Guid.NewGuid().ToString();
            sesion.FechaCreacionToken = DateTime.Now;
            sesion.EstadoStep = "Inicio";
            return $"Ingrese al siguente link para registrar la nueva tarea: https://4cmlk6kl-7045.usw3.devtunnels.ms/FormulariosSupervisor/CrearTarea?id={proyecto.Id}&token={sesion.TokenFormularios}";
        }
    }
}
