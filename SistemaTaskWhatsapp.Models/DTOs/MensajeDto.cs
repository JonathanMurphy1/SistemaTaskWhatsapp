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
    public class MensajeResponseDto
    {
        public int Id { get; set; }

        public Roles Tipo { get; set; }
        public string Contenido { get; set; }
        public string Cron { get; set; }

        public bool Activo { get; set; }

        public DateTime FechaCreacion { get; set; }

        public int EmpresaId { get; set; }

    }
}
