using Microsoft.AspNetCore.Identity;
using SistemaTaskWhatsapp.Utilidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Models
{
    public class EvidenciaDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Descripcion { get; set; }
        public int ReporteId { get; set; }
    }
}
