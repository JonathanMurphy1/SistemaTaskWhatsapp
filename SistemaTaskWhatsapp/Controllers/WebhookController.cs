using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Services;
using System.Threading.Tasks;
using Twilio.TwiML;

namespace SistemaTaskWhatsapp.Areas.WhatsApp.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly WhatsAppService _whatsapp;
        private readonly WhatsAppFlowService _whatsAppFlowService;
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public WebhookController(WhatsAppService whatsApp, IContenedorTrabajo contenedorTrabajo, WhatsAppFlowService whatsAppFlowService)
        {
            _whatsapp = whatsApp;
            _contenedorTrabajo = contenedorTrabajo;
            _whatsAppFlowService = whatsAppFlowService;
        }

        [HttpPost("Receive")]
        public async Task<IActionResult> Receive()
        {
            var from = Request.Form["From"].ToString().Replace("whatsapp:", "");
            string body = Request.Form["Body"].ToString().Trim();

            int numMedia = 0;
            int.TryParse(Request.Form["NumMedia"], out numMedia);

            if (numMedia > 0)
            {
                body = Request.Form["MediaUrl0"].ToString();
            }

            string respuesta = await _whatsAppFlowService.ProcesarMensajeAsync(from, body);
            var twiml = new Twilio.TwiML.MessagingResponse();
            twiml.Message(respuesta);

            return Content(twiml.ToString(), "application/xml");
        }
    }
}