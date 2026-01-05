using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Models.ViewModels
{
    public class ProyectoHomeVM
    {
        public Proyecto? Proyecto { get; set; }

        public int TareasPendientes { get; set; }
        public int ReportesPendientesRevisar { get; set; }
    }
}
