using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Models
{
    [Table("EmpresaPrograma")]
    public class EmpresaPrograma
    {
        public int Id { get; set; }

        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; }

        public int ProgramaId { get; set; }
        public Programa Programa { get; set; }
    }
}
