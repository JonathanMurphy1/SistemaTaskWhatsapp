using SistemaTaskWhatsapp.Models;
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
    [Table("Mensaje")]
    public class Mensaje
    {
        [Key]
        public int Id { get; set; }

        public Roles Tipo { get; set; }  //Define a quien se envia empleados / supervisores 
        public string Contenido { get; set; }

       public string Cron { get; set; } // "10 9 * * 1-5"

       public bool Activo { get; set; }

       public DateTime FechaCreacion { get; set; }

       public int? EmpresaId { get; set; }
       public Empresa? Empresa { get; set; }

    }
}

