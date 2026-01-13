using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Models.ViewModels;
using SistemaTaskWhatsapp.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

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

            var state = SupervisorStateFactory.Create(
                sesion.EstadoStep,
                _contenedorTrabajo
                );

            respuesta = await state.HandleAsync(
                usuario,
                sesion,
                mensaje
                );

            sesion.FechaActualizacion = DateTime.Now;
            _contenedorTrabajo.ChatSession.Update(sesion);
            await _contenedorTrabajo.SaveAsync();
            return respuesta;
        }
    }
}
