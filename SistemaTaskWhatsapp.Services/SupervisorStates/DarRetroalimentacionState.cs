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
    internal class DarRetroalimentacionState : ISupervisorState
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public DarRetroalimentacionState(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }
        public async Task<string> HandleAsync(Usuario usuario, ChatSession sesion, string mensaje)
        {
            string mensajeLower = mensaje.ToLower();
            if (mensajeLower != "si")
            {
                sesion.EstadoStep = "DetalleReporte";
                return "Escriba el Id de otro reporte que quiera revisar.\n" +
                    "Escriba *Inicio* para volver al menú principal";
            }

            sesion.EstadoStep = "AceptarReporte";
            return "Escriba sus comentarios acerca de los avances y el reporte";
        }
    }
}
