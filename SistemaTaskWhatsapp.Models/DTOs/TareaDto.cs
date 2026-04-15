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
    public class TareaDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int? Estado { get; set; }
        public string? ProjectName { get; set; }
        public int SubtaskId { get; set; }
    }

}
