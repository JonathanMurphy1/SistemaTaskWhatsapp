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
    public class EmpresaCreateDto
    {
        public string Nombre { get; set; }
        public int? IdExterno { get; set; }
        public int ProgramaOrigenId { get; set; }
    }

    public class EmpresaResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int? IdExterno { get; set; }
        public int ProgramaOrigenId { get; set; }
    }
}
