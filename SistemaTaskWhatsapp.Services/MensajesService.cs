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

        public MensajesService(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        // Recordatorio de tareas para empleados
        public async Task EnviarRecordatoriosTareas()
        {
            var empleados = await _contenedorTrabajo.Empleado
                .GetAllAsync(includeProperties: "Usuario");

            var tareasEmpleado = await _contenedorTrabajo.TareaEmpleado
                .GetAllAsync(includeProperties: "Tarea");

            foreach (var empleado in empleados)
            {
                int pendientes = tareasEmpleado
                    .Where(t => t.EmpleadoId == empleado.Id &&
                                t.Tarea.Estado == EstadosTarea.Pendiente)
                    .Count();

                string mensaje = string.Format(
                    SystemMessages.RecordatorioTareas,
                    empleado.Nombre,
                    pendientes
                );

                string telefono = empleado.Usuario?.PhoneNumber ?? "SIN TELEFONO";

                EnviarWhatsApp(telefono, mensaje);
            }
        }

        // Aviso a supervisores sobre reportes pendientes
        public async Task AvisarSupervisoresReportes()
        {
            var supervisores = await _contenedorTrabajo.Supervisor
                .GetAllAsync(includeProperties: "Usuario");

            var reportesPendientes = await _contenedorTrabajo.Reporte
                .GetAllAsync(
                    filter: r => r.Estado == EstadosReporte.PendienteRevisar,
                    includeProperties: "Empleado"
                );

            int totalPendientes = reportesPendientes.Count();

            foreach (var supervisor in supervisores)
            {
                string mensaje = string.Format(
                    SystemMessages.AvisoSupervisorReportes,
                    supervisor.Nombre,
                    totalPendientes
                );

                string telefono = supervisor.Usuario?.PhoneNumber ?? "SIN TELEFONO";

                EnviarWhatsApp(telefono, mensaje);
            }
        }

        // Simulación del envío
        private void EnviarWhatsApp(string telefono, string mensaje)
        {
            Console.WriteLine($"Mensaje enviado a {telefono}: {mensaje}");
        }
    }
}