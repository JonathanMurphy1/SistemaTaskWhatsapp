using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Utilidades;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Services
{
    public class MensajesService
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly WhatsAppService _whatsAppService;

        public MensajesService(
            IContenedorTrabajo contenedorTrabajo,
            WhatsAppService whatsAppService)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _whatsAppService = whatsAppService;
        }

        // Recordatorio de tareas para empleados
        public async Task EnviarRecordatoriosTareas()
        {

            var empleados = await _contenedorTrabajo.Empleado
                 .GetAllAsync(
                     filter: e => e.Estado == EstadosEmpleado.Activo,
                     includeProperties: "Usuario"
                 );

            var tareasEmpleado = await _contenedorTrabajo.TareaEmpleado.GetAllAsync(includeProperties: "Tarea");

            foreach (var empleado in empleados)
            {

                int pendientes = tareasEmpleado
                    .Where(t => t.EmpleadoId == empleado.Id &&
                                t.Tarea.Estado == EstadosTarea.Pendiente)
                    .Count();

                string mensaje;

                if (pendientes > 0)
                {
                    mensaje = string.Format(
                        SystemMessages.RecordatorioTareas,
                        empleado.Nombre,
                        pendientes
                    );
                }
                else
                {
                    mensaje = string.Format(
                        SystemMessages.SinTareas,
                        empleado.Nombre
                    );
                }

                string telefono = empleado.Usuario?.PhoneNumber;

                if (!string.IsNullOrEmpty(telefono))
                {
                    await EnviarWhatsApp(telefono, mensaje);


                }
            }
        }

        // Aviso a supervisores
        public async Task AvisarSupervisoresReportes()
        {
            var supervisores = await _contenedorTrabajo.Supervisor
                .GetAllAsync(
                    filter: s => s.Estado == EstadosSupervisor.Activo,
                    includeProperties: "Usuario"
                );

            int totalPendientes = (await _contenedorTrabajo.Reporte
                        .GetAllAsync(
                            filter: r => r.Estado == EstadosReporte.PendienteRevisar
                        )).Count();

            foreach (var supervisor in supervisores)
            {
                string mensaje;

                if (totalPendientes > 0)
                {
                    mensaje = string.Format(
                        SystemMessages.AvisoSupervisorReportes,
                        supervisor.Nombre,
                        totalPendientes
                    );
                }
                else
                {
                    mensaje = string.Format(
                        SystemMessages.SinReportes,
                        supervisor.Nombre
                    );
                }

                string telefono = supervisor.Usuario?.PhoneNumber;

                if (!string.IsNullOrEmpty(telefono))
                {
                    await EnviarWhatsApp(telefono, mensaje);
                }
            }
        }

        // Envío de WhatsApp
        private async Task EnviarWhatsApp(string telefono, string mensaje)
        {
            try
            {
                if (!telefono.StartsWith("+"))
                {
                    telefono = $"+521{telefono}";
                }
                
                //Console.WriteLine($"Mensaje enviado a {telefono}: {mensaje}");
                await _whatsAppService.EnviarMensajeAsync(telefono, mensaje);
            
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enviando mensaje a {telefono}: {ex.Message}");
            }
        }
    }
}