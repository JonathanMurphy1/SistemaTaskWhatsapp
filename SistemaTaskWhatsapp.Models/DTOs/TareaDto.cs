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
    public class TareaCreateDto
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaEntrega { get; set; }

        public int Estado { get; set; }

        public int? ProyectoId { get; set; }

        public int? IdExterno { get; set; }

        public int ProgramaId { get; set; }
    }

    public class TareaResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime? FechaTermino { get; set; }
        public DateTime? FechaEntrega { get; set; }

        public int Estado { get; set; }

        public int ProyectoId { get; set; }

        public int? IdExterno { get; set; }
    }
}
