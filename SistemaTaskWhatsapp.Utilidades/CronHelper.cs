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
            var dias = partes[4];

            string frecuencia = dias switch
            {
                "*" => "Todos los días",
                "1-5" => "Lunes a viernes",
                "1" => "Cada lunes",
                _ => "Frecuencia personalizada"
            };

            // Formato de hora
            if (int.TryParse(hora, out int h) && int.TryParse(minuto, out int m))
            {
                var time = new TimeOnly(h, m);
                return $"{frecuencia} a las {time.ToString("hh:mm tt")}";
            }

            return cron;
        }
    }
}
