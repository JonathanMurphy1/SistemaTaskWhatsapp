using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Models.ViewModels
{
    public class EvidenciaVM
    {
        public Evidencia? Evidencia { get; set; }

        public IEnumerable<SelectListItem>? ListaReportes { get; set; }
    }
}
