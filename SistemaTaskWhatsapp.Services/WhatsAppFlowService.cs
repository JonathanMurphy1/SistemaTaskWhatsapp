using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Services
{
    public class WhatsAppFlowService
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        //private readonly UserManager<Usuario> _userManager;
        private readonly IConfiguration _config;
        private readonly string _baseUrl;

        public WhatsAppFlowService(
            IContenedorTrabajo contenedorTrabajo,
            /*UserManager<Usuario> userManager,*/
            IConfiguration config)
        {
            _contenedorTrabajo = contenedorTrabajo;
            //_userManager = userManager;
            _config = config;

            _baseUrl = _config["BaseUrl"];
        }

        [HttpGet]
        public async Task<string> ProcesarMensajeAsync(
            string from,
            string body
            )
        {
            //Formatear el telefono de twilio
            string? telefonoFormateado = null;

            if (from.Length > 10)
            {
                telefonoFormateado = from.Substring(from.Length - 10);
            }

            var mensaje = body.Trim();
            var mensajeMayus = body.Trim().ToUpper();
            var supervisor = await _contenedorTrabajo.Supervisor.GetFirstOrDefaultAsync(c => c.Usuario.Telefono == telefonoFormateado);
            var empleado = await _contenedorTrabajo.Empleado.GetFirstOrDefaultAsync(o => o.Usuario.Telefono == telefonoFormateado);
            //var sesion = await _contenedorTrabajo.ChatSession.GetFirstOrDefaultAsync(s => s.Numero == from);

            string respuesta = "Ya entra en el servicio\n" +
                $"Escribiste esto: {mensaje}\n" +
                $"Desde: {telefonoFormateado}";

            return respuesta;
        }
    }
}
