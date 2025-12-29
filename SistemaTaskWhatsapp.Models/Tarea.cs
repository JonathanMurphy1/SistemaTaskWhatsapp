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
    [Table("Tarea")]
    public class Tarea
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Escriba el nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Elija el proyecto")]
        public int ProyectoId { get; set; }
        [ForeignKey("ProyectoId")]
        public Proyecto? Proyecto { get; set; }

        [Required(ErrorMessage = "Escriba la descripción")]
        public string Descripcion { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime? FechaTermino { get; set; }

        [Required(ErrorMessage = "La fecha de entrega es obligatoria")]
        public DateTime? FechaEntrega { get; set; }

        public EstadosTarea Estado { get; set; }
    }
}
