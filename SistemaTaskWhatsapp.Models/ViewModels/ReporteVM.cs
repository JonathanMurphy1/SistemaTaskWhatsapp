using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Models.ViewModels
{
    public class ReporteVM
    {
        public Reporte? Reporte { get; set; }

        public IEnumerable<SelectListItem>? ListaTareas { get; set; }

        public IEnumerable<SelectListItem>? ListaEmpleados { get; set; }
    }
}
