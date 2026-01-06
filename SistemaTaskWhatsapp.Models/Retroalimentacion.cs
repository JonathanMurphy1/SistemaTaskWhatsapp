using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Models
{
    [Table("Retroalimentacion")]
    public class Retroalimentacion
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El comentario es obligatorio")]
        public string Comentario { get; set; }

        public DateTime Fecha { get; set; }

        public bool VistoEmpleado { get; set; } = false;

        public int? SupervisorId { get; set; }
        [ForeignKey("SupervisorId")]
        public Supervisor? Supervisor { get; set; }

        [Required(ErrorMessage = "El reporte es obligatorio")]
        public int? ReporteId { get; set; }
        [ForeignKey("ReporteId")]
        public Reporte? Reporte { get; set; }
    }
}
