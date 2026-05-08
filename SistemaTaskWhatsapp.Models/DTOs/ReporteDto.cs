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
    public class ReporteResponseDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; }
        public string Contenido { get; set; }
        public string? Inconvenientes { get; set; }
        public string? ComentarioEmpleado { get; set; }

        public string Estado { get; set; }
        public DateTime FechaSubida { get; set; }

        public int? TareaId { get; set; }
        public string TareaNombre { get; set; }

        public int? EmpleadoId { get; set; }
        public string EmpleadoNombre { get; set; }
    }
}
