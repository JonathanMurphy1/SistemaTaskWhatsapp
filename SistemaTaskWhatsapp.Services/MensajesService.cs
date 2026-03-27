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

        //Funcion para obtener días festivos mediante una API y guardarlos en la BD
        private async Task<List<DiaFestivo>> ObtenerFestivos()
        {
            int year = DateTime.Now.Year;

            // Buscar en BD al iniciar
            var festivosBD = await _contenedorTrabajo.DiaFestivo.GetAllAsync(f => f.Date.Year == year);

            if (festivosBD != null && festivosBD.Any())
            {
                return festivosBD.ToList();
            }

            // Si no hay días guardados en BD llamar a la API
            using (var http = new HttpClient())
            {
                var url = $"https://date.nager.at/api/v3/PublicHolidays/{year}/MX";
                var response = await http.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Error al consultar API");
                    return new List<DiaFestivo>();
                }

                var json = await response.Content.ReadAsStringAsync();

                var festivosApi = System.Text.Json.JsonSerializer.Deserialize<List<DiaFestivo>>(json,
                    new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                // Guardar en BD para no depender siempre de la API
                foreach (var festivo in festivosApi)
                {
                    await _contenedorTrabajo.DiaFestivo.AddAsync(festivo);
                }

                await _contenedorTrabajo.SaveAsync();

                return festivosApi;
            }
        }

        //Revisa si el día actual es festivo o no
        private async Task<bool> EsDiaFestivo()
        {
            var festivos = await ObtenerFestivos();
            var hoy = DateTime.Now.Date;

            return festivos.Any(f => f.Date.Date == hoy);

        }

        // Recordatorio de tareas para empleados
        public async Task EnviarRecordatoriosTareas()
        {
            //Función para revisar si el día es festivo, si lo es no manda mensaje
            if (await EsDiaFestivo())
            {
                //Console.WriteLine("Hoy es festivo (API), no se envían mensajes");
                return;
            }

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

                //Si tiene pendientes se los recuerda
                if (pendientes > 0)
                {
                    mensaje = string.Format(
                        SystemMessages.RecordatorioTareas,
                        empleado.Nombre,
                        pendientes
                    );
                }
                //Si no tiene pendientes se lo notifica
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
            if (await EsDiaFestivo())
            {
                //Console.WriteLine("Hoy es festivo (API), no se envían mensajes");
                return;
            }

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

                //Si tiene pendientes se los recuerda
                if (totalPendientes > 0)
                {
                    mensaje = string.Format(
                        SystemMessages.AvisoSupervisorReportes,
                        supervisor.Nombre,
                        totalPendientes
                    );
                }
                //Si no tiene pendientes se lo notifica
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
                //Los numeros en la base de datos no tiene el +521 aquí se agrega para enviar el mensaje
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