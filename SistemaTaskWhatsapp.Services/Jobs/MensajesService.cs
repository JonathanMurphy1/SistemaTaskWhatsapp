using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Services.Jobs
{
    public class MessageJobs
    {
        private readonly MensajesService _mensajesService;

        public MessageJobs(MensajesService mensajesService)
        {
            _mensajesService = mensajesService;
        }

        // Job para recordatorios a empleados
        public async Task EnviarRecordatoriosEmpleados()
        {
            await _mensajesService.EnviarRecordatoriosTareas();
        }

        // Job para avisar a supervisores
        public async Task AvisarSupervisores()
        {
            await _mensajesService.AvisarSupervisoresReportes();
        }
    }
}