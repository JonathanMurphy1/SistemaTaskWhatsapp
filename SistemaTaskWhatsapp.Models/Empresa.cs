using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Models
{
    [Table("Empresa")]
    public class Empresa
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Escriba el nombre por favor")]
        public string Nombre { get; set; }

        public DateTime FechaRegistro { get; set; }

        public int CompaniesId { get; set; }

        public IEnumerable<Proyecto>? Proyectos { get; set; }
    }
}
