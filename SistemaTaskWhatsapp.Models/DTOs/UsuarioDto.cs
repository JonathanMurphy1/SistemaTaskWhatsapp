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
    public class UsuarioResponseDto
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }

        public string Rol { get; set; }

        public int? EmpresaId { get; set; }
        public string EmpresaNombre { get; set; }

        public int ProgramaId { get; set; }

        public string ProgramaNombre { get; set; }

        public int? IdExterno { get; set; }

    }

    public class UsuarioCreateDto
    {
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string? PhoneNumber { get; set; }

        public int UserTypeId { get; set; }

        public int? EmpresaId { get; set; }

        public int ProgramaId { get; set; }

        public int? IdExterno { get; set; }
    }

    public class UsuarioUpdateDto
    {
        public int IdExterno { get; set; }

        public string Nombre { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }

        public int? UserTypeId { get; set; }

        public int? ProgramaId { get; set; }

        public int? EmpresaId { get; set; }
    }

}
