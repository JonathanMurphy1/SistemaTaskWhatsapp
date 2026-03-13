using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Services.Jobs
{
    public class MessageJobs
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public MessageJobs(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        // Job para recordatorios a empleados
        public async Task EnviarRecordatoriosEmpleados()
        {
            MensajesService servicio = new MensajesService(_contenedorTrabajo);

            await servicio.EnviarRecordatoriosTareas();
        }

        // Job para avisar a supervisores
        public async Task AvisarSupervisores()
        {
            MensajesService servicio = new MensajesService(_contenedorTrabajo);

            await servicio.AvisarSupervisoresReportes();
        }
    }
}