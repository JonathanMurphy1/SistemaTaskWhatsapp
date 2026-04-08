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

        //Función para cargar los mensajes y enviarlos
        public async Task EnviarMensajeProgramado(int mensajeId)
        {
            var mensaje = await _contenedorTrabajo.Mensaje
                .GetByIdAsync(mensajeId);

            if (mensaje == null || !mensaje.Activo)
                return;

            //Validar día festivo
            if (await EsDiaFestivo())
                return;

            switch (mensaje.Tipo)
            {
                case Roles.Empleado:
                    await EnviarMensajeAEmpleados(mensaje);
                    break;

                case Roles.Supervisor:
                    await EnviarMensajeASupervisores(mensaje);
                    break;
            }
        }

        //Función para enviar mensajes unicamente a los empleados activos de la empresa correspondiente
        private async Task EnviarMensajeAEmpleados(Mensaje mensaje)
        {
            var empleados = await _contenedorTrabajo.Empleado
                .GetAllAsync(
                    filter: e => e.Estado == EstadosEmpleado.Activo
                                && e.Usuario.EmpresaId == mensaje.EmpresaId,
                    includeProperties: "Usuario"
                );

            foreach (var empleado in empleados)
            {
                string texto = mensaje.Contenido;

                string telefono = empleado.Usuario?.PhoneNumber;

                if (!string.IsNullOrEmpty(telefono))
                {
                    await EnviarWhatsApp(telefono, texto);
                }
            }
        }

        //Función para enviar mensajes unicamente a los supervisores activos de la empresa correspondiente
        private async Task EnviarMensajeASupervisores(Mensaje mensaje)
        {
            var supervisores = await _contenedorTrabajo.Supervisor
                .GetAllAsync(
                    filter: s => s.Estado == EstadosSupervisor.Activo
                                && s.Usuario.EmpresaId == mensaje.EmpresaId,
                    includeProperties: "Usuario"
                );

            foreach (var supervisor in supervisores)
            {
                string texto = mensaje.Contenido;

                string telefono = supervisor.Usuario?.PhoneNumber;

                if (!string.IsNullOrEmpty(telefono))
                {
                    await EnviarWhatsApp(telefono, texto);
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