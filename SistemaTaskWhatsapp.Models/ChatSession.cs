using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Models
{
    [Table("ChatSession")]
    public class ChatSession
    {
        [Key]
        public int Id { get; set; }

        public string Numero { get; set; }

        public string EstadoStep { get; set; }

        public string? DatosParciales { get; set; }

        public DateTime FechaActualizacion { get; set; } = DateTime.Now;
    }
}
