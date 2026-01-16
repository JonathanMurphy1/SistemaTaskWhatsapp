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
    internal class AceptarReporteState : ISupervisorState
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public AceptarReporteState(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }
        public async Task<string> HandleAsync(Usuario usuario, ChatSession sesion, string mensaje)
        {
            var data = SessionJsonHelper.GetData<RevisarVM>(sesion.DatosParciales);
            data.Comentario = mensaje;

            sesion.DatosParciales = SessionJsonHelper.SetData(data);

            sesion.EstadoStep = "GuardarRetroalimentacion";

            return "¿Deseas aceptar este reporte?\n" +
                "*Si*.Aceptar\n" +
                "*No*.Rechazar";
        }
    }
}
