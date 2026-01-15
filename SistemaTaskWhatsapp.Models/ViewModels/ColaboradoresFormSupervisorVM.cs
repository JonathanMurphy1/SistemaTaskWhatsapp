using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Models.ViewModels
{
    public class ColaboradoresFormSupervisorVM
    {
        public Tarea? Tarea { get; set; }

        public string? Token { get; set; }

        public IEnumerable<SelectListItem>? ListaEmpleados { get; set; }
    }
}
