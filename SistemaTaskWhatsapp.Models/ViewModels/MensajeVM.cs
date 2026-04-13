using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Models.ViewModels
{
    public class MensajeVM
    {
        public Mensaje? Mensaje { get; set; }

        public IEnumerable<SelectListItem>? ListaEmpresas { get; set; }

        public IEnumerable<SelectListItem>? ListaFestivos { get; set; }

        public List<int> FestivosSeleccionados { get; set; } = new();

        //Crear dias festivos desde la vista
        public List<DiaFestivo> NuevosFestivos { get; set; } = new();

    }
}
