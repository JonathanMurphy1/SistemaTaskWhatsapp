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
    internal class GuardarRetroalimentacionReporteState : ISupervisorState
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public GuardarRetroalimentacionReporteState(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        public async Task<string> HandleAsync(Usuario usuario, ChatSession sesion, string mensaje)
        {
            var data = SessionJsonHelper.GetData<RevisarVM>(sesion.DatosParciales);
            string mensajeLower = mensaje.ToLower();
            var supervisor = await _contenedorTrabajo.Supervisor.GetFirstOrDefaultAsync(s => s.UsuarioId == usuario.Id);

            if (mensajeLower == "si") data.EstadoReporte = EstadosReporte.Aceptado;
            else if (mensajeLower == "no") data.EstadoReporte = EstadosReporte.Rechazado;
            else
            {
                return "Opción no valida, elija una de las dos o escriba *Inicio* para volver al menú de inicio";
            }

            var nuevaRetroalimentacion = new Retroalimentacion
            {
                Comentario = data.Comentario,
                Fecha = DateTime.Now,
                SupervisorId = supervisor.Id,
                ReporteId = data.ReporteId,
            };

            await _contenedorTrabajo.Retroalimentacion.AddAsync(nuevaRetroalimentacion);

            var reporte = await _contenedorTrabajo.Reporte.GetByIdAsync(data.ReporteId);
            reporte.Estado = data.EstadoReporte;
            sesion.EstadoStep = "Inicio";
            sesion.DatosParciales = "";
            _contenedorTrabajo.Reporte.Update(reporte);
            return "Retroalimentación enviada correctamente, volviendo al inicio";
        }
    }
}
