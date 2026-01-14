using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Services.SupervisorStates;
using SistemaTaskWhatsapp.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Services
{
    public static class SupervisorStateFactory
    {
        public static ISupervisorState Create(
            string estado,
            IContenedorTrabajo ct
            )
        {
            return estado switch
            {
                SupervisorSteps.Inicio => new InicioState(ct),
                SupervisorSteps.Menu => new MenuState(ct),
                SupervisorSteps.DetalleReporte => new DetalleReporteState(ct),
                SupervisorSteps.DarRetroalimentacion => new DarRetroalimentacionState(ct),
                SupervisorSteps.AceptarReporte => new AceptarReporteState(ct),
                SupervisorSteps.GuardarRetroalimentacion => new GuardarRetroalimentacionReporteState(ct),
                SupervisorSteps.EleccionTarea => new EleccionTareaState(ct),
                SupervisorSteps.RespuestaCrearTarea => new RespuestaCrearTareaState(ct),
                _ => new ErrorState(ct)
            };
        }
    }
}
