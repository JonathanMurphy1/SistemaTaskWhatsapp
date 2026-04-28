using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Models.ViewModels
{
    public class EmpresaProgramaVM
    {
        public int Id { get; set; }

        public int EmpresaId { get; set; }
        public string EmpresaNombre { get; set; }

        public int ProgramaId { get; set; }
        public string ProgramaNombre { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}
