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
    public class ProjectDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int EmpresaId { get; set; }
        public int Estado { get; set; }
    }
}
