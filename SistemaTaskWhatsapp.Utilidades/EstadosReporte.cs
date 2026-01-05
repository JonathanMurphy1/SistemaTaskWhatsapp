using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Utilidades
{
    public enum EstadosReporte
    {
        [Display(Name = "Pendiente de revisar")]
        PendienteRevisar = 0,
        Aceptado = 1,
        Rechazado = 2,
    }
}
