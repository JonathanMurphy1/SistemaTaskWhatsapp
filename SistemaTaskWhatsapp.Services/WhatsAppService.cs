using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace SistemaTaskWhatsapp.Services
{
    public class WhatsAppService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _twilioNumber;

        public WhatsAppService(string accountSid, string authToken, string twilioNumber)
        {
            _accountSid = accountSid;
            _authToken = authToken;
            _twilioNumber = twilioNumber;

            TwilioClient.Init(_accountSid, _authToken);
        }

        public async Task<string> EnviarMensajeAsync(string numeroDestino, string mensaje)
        {
            try
            {
                //TwilioClient.Init(_accountSid, _authToken, _twilioNumber);
                TwilioClient.Init(_accountSid, _authToken);

                var message = await MessageResource.CreateAsync(
                    
                    //from: new PhoneNumber($"whatsapp:{_twilioNumber}"),
                    from: new PhoneNumber("whatsapp:+14155238886"), 
                    to: new PhoneNumber($"whatsapp:{numeroDestino}"),
                    body: mensaje
                );

                return $"Mensaje enviado correctamente (SID: {message.Sid}";
            }
            catch(Exception ex)
            {
                return $"Error al enviar mensaje: {ex.Message}";
            }
        }
    }
}
