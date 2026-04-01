using Microsoft.AspNetCore.Identity;
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
    public class UsuarioUpdateDto
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        //Opcional debido a que aveces se tomar en cuenta para editar
        public string? PhoneNumber { get; set; }
        public int? UserTypeId { get; set; }
    }

    public class UsuarioDeleteDto
    {
        public int UserId { get; set; }
    }

}
