using Microsoft.AspNetCore.Mvc.Rendering;
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
        [Required(ErrorMessage = "Escriba el nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Escriba el Email")]
        [EmailAddress(ErrorMessage = "El email no es válido")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Escriba la contraseña")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Escriba el teléfono")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "El teléfono solo deben de ser 10 números.")]
        public string? Telefono { get; set; }

        [Required(ErrorMessage = "Seleccione un rol")]
        public Roles Rol { get; set; }

        public Supervisor? Supervisor { get; set; }

        public Empleado? Empleado { get; set; }

        [Required(ErrorMessage = "Seleccione una empresa")]
        public int? EmpresaId { get; set; }


        [Required(ErrorMessage = "Seleccione un programa")]
        public int? ProgramaId { get; set; }

        public IEnumerable<SelectListItem>? ListaProgramas { get; set; }
        public IEnumerable<SelectListItem>? ListaEmpresas { get; set; }
    }
}
