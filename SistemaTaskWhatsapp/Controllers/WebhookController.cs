using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Services;
using System.Threading.Tasks;
using Twilio.TwiML;

namespace SistemaTaskWhatsapp.Areas.WhatsApp.Controllers
{
    [Area("WhatsApp")]
    [Route("Whatsapp/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly WhatsAppService _whatsapp;
       // private readonly WhatsAppFlowService _whatsAppFlowService;
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public WebhookController(WhatsAppService whatsApp, IContenedorTrabajo contenedorTrabajo  /*, WhatsAppFlowService whatsAppFlowService*/)
        {
            _whatsapp = whatsApp;
            _contenedorTrabajo = contenedorTrabajo;
            //_whatsAppFlowService = whatsAppFlowService;
        }

        [HttpPost("Receive")]
        public async Task<IActionResult> Receive()
        {
            Console.WriteLine("Hola ya entro al metodo");
            var from = Request.Form["From"].ToString().Replace("whatsapp:", "");
            var body = Request.Form["Body"].ToString().Trim();

            int numMedia = 0;
            int.TryParse(Request.Form["NumMedia"], out numMedia);

            string mediaUrl = null;
            string mediaContentType = null;

            if (numMedia > 0)
            {
                mediaUrl = Request.Form["MediaUrl0"].ToString();
                mediaContentType = Request.Form["MediaContentType0"].ToString();
            }

            var latitude = Request.Form["Latitude"].ToString();
            var longitude = Request.Form["Longitude"].ToString();
            var address = Request.Form["Address"].ToString();

            //string respuesta = await _whatsAppFlowService.ProcesarMensajeAsync(
            //    from,
            //    body,
            //    latitude,
            //    longitude,
            //    address,
            //    mediaUrl,
            //    mediaContentType
            //);

            string respuesta = "Hola mundo";
            var twiml = new Twilio.TwiML.MessagingResponse();
            twiml.Message(respuesta);

            return Content(twiml.ToString(), "application/xml");
        }
    }
}