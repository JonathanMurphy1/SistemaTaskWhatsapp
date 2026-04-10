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
    [Table("MensajeDiaFestivo")]
    public class MensajeDiaFestivo
    {
        public int Id { get; set; }
        public int MensajeId { get; set; }
        public Mensaje Mensaje { get; set; }

        public int DiaFestivoId { get; set; }
        public DiaFestivo DiaFestivo { get; set; }
    }
}
