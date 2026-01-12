using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Data;
using SistemaTaskWhatsapp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Services
{
    public class EmpleadoFlowService : IEmpleadoFlowService
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public EmpleadoFlowService(IContenedorTrabajo contenedorTrabajo) 
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        public async Task<string> ProcesarAsync(
            Usuario usuario,
            ChatSession sesion,
            string mensaje
            )
        {
            switch (sesion.EstadoStep)
            {
                case "Inicio":
                    sesion.EstadoStep = "Menu";
                    await _contenedorTrabajo.SaveAsync();
                    return "Hola empleado 1- Ver tareas 2-reportar avance";
                
                case "Menu":
                    if (mensaje == "1")
                        return "Estas son tus tareas";
                    if (mensaje == "2")
                        return "Escribe tu reporte";
                    break;
            }
            return "Opción no valida";

        }
    }
}
