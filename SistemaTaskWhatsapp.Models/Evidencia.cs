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
    [Table("Evidencia")]
    public class Evidencia
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ingrese la URL de la evidencia")]
        [Url(ErrorMessage = "Ingrese una URL válida")]
        public string Url { get; set; }

        [Required(ErrorMessage = "Escriba la descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Elija el proyecto")]
        public int ReporteId { get; set; }

        [ForeignKey("ReporteId")]
        public Reporte? Reporte { get; set; }

    }
}
