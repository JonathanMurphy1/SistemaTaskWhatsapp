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
    [Table("DiaFestivo")]
    public class DiaFestivo
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string LocalName { get; set; }
    }
}
