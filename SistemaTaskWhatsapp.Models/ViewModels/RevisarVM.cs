using SistemaTaskWhatsapp.Utilidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Models.ViewModels
{
    public class RevisarVM
    {
        [Required]
        public int TareaId { get; set; }
        [Required]
        public int ReporteId { get; set; }
        [Required]
        public string Comentario { get; set; }
        [Required]
        public EstadosReporte EstadoReporte { get; set; }
    }
}
