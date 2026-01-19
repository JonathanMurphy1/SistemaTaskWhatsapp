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
    public class Empleado
    {
        [Key]
        public int Id { get; set; }

        public string? UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]

        public Usuario? Usuario { get; set; }
       
        public string? Nombre { get; set; }

        public DateTime FechaRegistro { get; set; }

        public EstadosEmpleado Estado { get; set; }

        public IEnumerable<Tarea>? Tarea { get; set; }

        //Propiedades de navegacion
        public ICollection<TareaEmpleado>? TareasEmpleado { get; set; }
    }
}

