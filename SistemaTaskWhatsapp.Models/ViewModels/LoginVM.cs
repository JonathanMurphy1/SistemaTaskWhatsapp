using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Models.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Esciba su email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Escriba la contraseña")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
