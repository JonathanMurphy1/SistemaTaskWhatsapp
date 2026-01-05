using SistemaTaskWhatsapp.Utilidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Models.ViewModels
{
    public class UsuarioCreateVM
    {
        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "Seleccione un rol")]
        public Roles Rol { get; set; }

        public Supervisor? Supervisor { get; set; }

        public Empleado? Empleado { get; set; }

    }
}
