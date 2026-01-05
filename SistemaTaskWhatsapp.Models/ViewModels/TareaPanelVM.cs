using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Models.ViewModels
{
    public class TareaPanelVM
    {
        public Tarea? Tarea { get; set; }

        public int ReportesPendientesRevisar { get; set; }
    }
}
