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
    internal class ErrorState : ISupervisorState
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public ErrorState(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }
        public async Task<string> HandleAsync(Usuario usuario, ChatSession sesion, string mensaje)
        {
            sesion.EstadoStep = "Inicio";
            return "Hubo un problema, voviendo al menú de inicio";
        }
    }
}
