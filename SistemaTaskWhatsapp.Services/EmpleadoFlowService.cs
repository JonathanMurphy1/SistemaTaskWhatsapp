using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Data;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaTaskWhatsapp.Utilidades;

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
           string mensaje)
        {
            var empleado = await _contenedorTrabajo.Empleado.GetFirstOrDefaultAsync(e => e.UsuarioId == usuario.Id);

            if (empleado == null) return "No se encontro el empleado";

            if (empleado.Estado == EstadosEmpleado.Inactivo)
                return "Tu cuenta de empleado está inactiva";

            switch (sesion.EstadoStep)
            {
                //Inicio del flujo
                case "Inicio":
                    sesion.EstadoStep = "Menu";
                    await _contenedorTrabajo.SaveAsync();

                    var tareasPendientes = await _contenedorTrabajo.Tarea.GetAllAsync(t => t.Estado == EstadosTarea.Pendiente);
                    var retroalimentacionesPendientes = await _contenedorTrabajo.Retroalimentacion.GetAllAsync(r => r.VistoEmpleado == false);

                    return $"Buen dia {usuario.Nombre}\n" +
                        "---------------------------------------------\n" +
                        "Elija una opción\n" +
                        "*1*.Consultar tareas\n" +
                        "*2*.Enviar reporte\n" +
                        "*3*.Revisar retroalimentaciones pendientes\n" +
                        $"Hay *{tareasPendientes.Count()}* tareas sin revisar\n" +
                        $"Hay *{retroalimentacionesPendientes.Count()}* retroalimentaciones sin revisar";

                //Menu principal
                case "Menu":
                    sesion.EstadoStep = "Inicio";
                    await _contenedorTrabajo.SaveAsync();
                    switch (mensaje)
                    {
                        case "1":
                            return "Elejiste la opción 1";
                        case "2":
                            return "Elejiste la opción 2";
                        case "3":
                            return "Elejiste la opción 3";
                        default:
                            return "Opción no valida";
                    }
            }
            return "Opción no valida";
        }
    }
}
