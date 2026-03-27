using System;

namespace SistemaTaskWhatsapp.Utilidades
{
    public static class SystemMessages
    {
        // Recordatorio para empleados sobre tareas pendientes
        public const string RecordatorioTareas =
            "Buenos días {0}, tienes {1} tareas pendientes. Puedes revisarlas mediante el chat asistente.";

        public const string SinTareas =
            "Buenos días {0}, no tienes tareas pendientes. De ser necesario comunicate con su asesor.";


        // Aviso para supervisores cuando un empleado tiene reportes por revisar
        public const string AvisoSupervisorReportes =
            "Buen día {0}, tienes {1} reportes sin revisar.";

        public const string SinReportes =
            "Buen día {0}, hoy no tienes reportes por revisar.";

    }
}