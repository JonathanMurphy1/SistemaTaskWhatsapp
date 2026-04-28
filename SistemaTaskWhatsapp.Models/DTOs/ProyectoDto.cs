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
    public class ProyectoCreateDto
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int EmpresaId { get; set; }
        public int ProgramaId { get; set; }
        public int Estado { get; set; } // se convierte a enum
        public int? IdExterno { get; set; }
    }

    public class ProyectoResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaFin { get; set; }
        public int EmpresaId { get; set; }
        public int ProgramaId { get; set; }
        public int Estado { get; set; }
        public int? IdExterno { get; set; }
    }
}
