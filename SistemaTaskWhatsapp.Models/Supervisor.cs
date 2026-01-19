using SistemaTaskWhatsapp.Models;
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
    public class Supervisor
    {
        [Key]
        public int Id { get; set; }

        public string? UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]

        public Usuario? Usuario { get; set; }

        public string? Nombre { get; set; }

        public EstadosSupervisor Estado { get; set; }
    }
}

