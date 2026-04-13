using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Models.ViewModels;
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
        private readonly UtilidadesService _utilidadesService;

        public MensajesService(
            IContenedorTrabajo contenedorTrabajo,
            WhatsAppService whatsAppService,
            UtilidadesService utilidadesService)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _whatsAppService = whatsAppService;
            _utilidadesService = utilidadesService;
        }

        //Función para cargar los mensajes y enviarlos
        public async Task EnviarMensajeProgramado(int mensajeId)
        {
            var mensaje = await _contenedorTrabajo.Mensaje
                .GetByIdAsync(mensajeId);

            if (mensaje == null || !mensaje.Activo)
                return;

            //Validar día festivo
            if (await _utilidadesService.EsDiaFestivo(mensaje.Id))
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
                    includeProperties: "Usuario,Usuario.Empresa"
                );
    

            foreach (var empleado in empleados)
            {
                int pendientes = await _utilidadesService.ObtenerTareasPendientes(empleado.Id);

                var valores = new Dictionary<string, string>
                {
                    { "Nombre", empleado.Nombre },
                    { "Pendientes", pendientes.ToString() },
                    { "Empresa", empleado.Usuario?.Empresa?.Nombre
                                          ?? mensaje.Empresa?.Nombre ?? "" }
                };

                string texto = _utilidadesService.ProcesarPlantilla(
                    mensaje.Contenido,
                    valores
                );

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
                    includeProperties: "Usuario,Usuario.Empresa"
                );

            int pendientes = await _utilidadesService.ObtenerReportesPendientes();

            foreach (var supervisor in supervisores)
            {
                var valores = new Dictionary<string, string>
                {
                    { "Nombre", supervisor.Nombre },
                    { "Pendientes", pendientes.ToString() },
                    { "Empresa", supervisor.Usuario?.Empresa?.Nombre
                                            ?? mensaje.Empresa?.Nombre ?? "" }
                };

                string texto = _utilidadesService.ProcesarPlantilla(
                    mensaje.Contenido,
                    valores
                );

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