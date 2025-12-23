using SistemaTaskWhatsapp.Utilidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Models.ViewModels
{
    public class EditarUsuarioVM
    {
        [Required(ErrorMessage = "No se encontró el Id del usuario")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Escriba el nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Escriba el Email")]
        [EmailAddress(ErrorMessage = "El email no es válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Escriba la contraseña")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Escriba el teléfono")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "Elija un rol")]
        public Roles Rol { get; set; }
    }
}
