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
    [Table("Proyecto")]
    public class Proyecto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El estado es obligarorio")]
        public EstadosProyecto Estado { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        public string Descripcion { get; set; }

        public DateTime FechaRegistro { get; set; }

        public DateTime? FechaFin { get; set; }

        //Empresa que tiene el proyecto
        [Required(ErrorMessage = "Elija una empresa")]
        public int EmpresaId { get; set; }

        [ForeignKey("EmpresaId")]
        public Empresa? Empresa { get; set; }

        public int? ProjectId { get; set; }

    }
}
