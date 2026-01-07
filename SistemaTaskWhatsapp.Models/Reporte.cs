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
    [Table("Reporte")]
    public class Reporte
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "No se asigno la tarea a la que pertenece el reporte")]
        public int TareaId { get; set; }
        [ForeignKey("TareaId")]
        public Tarea? Tarea { get; set; }

        [Required(ErrorMessage = "El nombre del reporte es obligatorio")]
        public string Nombre { get; set; }

        public int EmpleadoId { get; set; }
        [ForeignKey("EmpleadoId")]
        public Empleado? Empleado { get; set; }

        [Required(ErrorMessage = "El contenido del reporte es obligatorio")]
        public string Contenido { get; set; }

        public string? Inconvenientes { get; set; }

        public string? ComentarioEmpleado { get; set; }

        [Required(ErrorMessage = "Elija un estado para este reporte")]
        public EstadosReporte? Estado { get; set; }

        public DateTime FechaSubida { get; set; }

        public ICollection<Evidencia>? Evidencias { get; set; }
    }
}
