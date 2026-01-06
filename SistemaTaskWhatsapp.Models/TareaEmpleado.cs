using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Models
{
    [Table("TareaEmpleado")]
    public class TareaEmpleado
    {
        [Key]
        public int Id { get; set; }

        public int EmpleadoId { get; set; }
        [ForeignKey("EmpleadoId")]
        public Empleado? Empleado { get; set; }

        public int TareaId { get; set; }
        [ForeignKey("TareaId")]
        public Tarea? Tarea { get; set; }
    }
}
