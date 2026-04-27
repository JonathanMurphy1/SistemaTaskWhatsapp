using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Models
{
    [Table("Empresa")]
    public class Empresa
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Escriba el nombre por favor")]
        public string Nombre { get; set; }

        public DateTime FechaRegistro { get; set; }

        public int? IdExterno { get; set; } //Id de referencia del otro programa

        public int ProgramaOrigenId { get; set; }  //Id de referencia de quien creo el programa
        
        [ValidateNever]
        public Programa ProgramaOrigen { get; set; }

        public ICollection<EmpresaPrograma>? EmpresaProgramas { get; set; }

        public ICollection<Proyecto>? Proyectos { get; set; }

        public ICollection<Usuario>? Usuarios { get; set; }
    }
}
