using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Utilidades
{
    public static class CronHelper
    {
        public static string Formatear(string cron)
        {
            if (string.IsNullOrEmpty(cron))
                return "";

            var partes = cron.Split(' ');

            if (partes.Length < 5)
                return cron;

            var minuto = partes[0];
            var hora = partes[1];
            var diaMes = partes[2];
            var mes = partes[3];
            var diaSemana = partes[4];


            string frecuencia = "";

            //Detectar tipo de cron
            if (diaMes != "*" && mes != "*")
            {
                //Anual (día y mes definidos)
                if (int.TryParse(diaMes, out int d) && int.TryParse(mes, out int mesNumero))
                {
                    var fecha = new DateTime(2000, mesNumero, d);
                    frecuencia = $"Cada {fecha.ToString("dd 'de' MMMM")}";
                }
                else
                {
                    frecuencia = "Fecha específica";
                }
            }
            else
            {
                frecuencia = diaSemana switch
                {
                    "*" => "Todos los días",
                    "1-5" => "Lunes a viernes",
                    "1" => "Cada lunes",
                    _ => "Frecuencia personalizada"
                };
            }

            // Formato de hora
            if (int.TryParse(hora, out int h) && int.TryParse(minuto, out int min))
            {
                var time = new TimeOnly(h, min);
                return $"{frecuencia} a las {time.ToString("hh:mm tt")}";
            }

            return cron;
        }
    }
}
