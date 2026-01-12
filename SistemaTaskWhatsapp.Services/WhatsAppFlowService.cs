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
using SistemaTaskWhatsapp.Utilidades;

namespace SistemaTaskWhatsapp.Services
{
    public class WhatsAppFlowService
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        //private readonly UserManager<Usuario> _userManager;
        private readonly IConfiguration _config;
        private readonly string _baseUrl;
        private readonly ISupervisorFlowService _supervisorFlowService;

        public WhatsAppFlowService(
            IContenedorTrabajo contenedorTrabajo,
            /*UserManager<Usuario> userManager,*/
            IConfiguration config,
            ISupervisorFlowService supervisorFlowService)
        {
            _contenedorTrabajo = contenedorTrabajo;
            //_userManager = userManager;
            _config = config;
            _supervisorFlowService = supervisorFlowService; 

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
            string respuesta = "";

            if (from.Length > 10)
            {
                telefonoFormateado = from.Substring(from.Length - 10);
            }

            var mensaje = body.Trim();
            var mensajeMayus = body.Trim().ToUpper();
            var usuario = await _contenedorTrabajo.Usuario.GetFirstOrDefaultAsync(u => u.Telefono == telefonoFormateado);

            //Si no esta registrado con su numero lanza un mensaje
            if (usuario == null)
            {
                respuesta = "No estas regristrado en el sistema";
                return respuesta;
            }

            var sesion = await _contenedorTrabajo.ChatSession.GetFirstOrDefaultAsync(s => s.Numero == from);

            //Si no tiene sesion la crea
            if(sesion == null)
            {
                sesion = new Models.ChatSession
                {
                    Numero = from,
                    EstadoStep = "Inicio"
                };

                await _contenedorTrabajo.ChatSession.AddAsync(sesion);
                await _contenedorTrabajo.SaveAsync();
            }

            if(usuario.Rol == Roles.Supervisor)
            {
                respuesta = "Eres supervisor";
                //De aqui mandalo a un servicio especifico para el supervisor
            }
            else if (usuario.Rol == Roles.Empleado)
            {
                respuesta = "Eres minion";
                //De aqui mandalo a un servicio especifico para el empleado
            }
            else
            {
                respuesta = "Hubo un error, rol no valido";
            }

                return respuesta;
        }
    }
}
