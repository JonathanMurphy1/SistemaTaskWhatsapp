using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaTaskWhatsapp.Utilidades;

namespace SistemaTaskWhatsapp.Services
{
    public class SupervisorFlowService : ISupervisorFlowService
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public SupervisorFlowService(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }
        public async Task<string> ProcesarAsync(
            Usuario usuario, 
            ChatSession sesion, 
            string mensaje)
        {
            var supervisor = await _contenedorTrabajo.Supervisor.GetFirstOrDefaultAsync(e => e.UsuarioId == usuario.Id);

            if (supervisor == null) return "No se encontro el supervisor";

            if (supervisor.Estado == EstadosSupervisor.Inactivo)
                return "Tu cuenta de supervisor esta inactiva";

            switch (sesion.EstadoStep)
            {
                //Inicio del flujo
                case "Inicio":
                    sesion.EstadoStep = "Menu";
                    await _contenedorTrabajo.SaveAsync();

                    var reportesPendientes = await _contenedorTrabajo.Reporte.GetAllAsync(r => r.Estado == EstadosReporte.PendienteRevisar);

                    return "Hola Supervisor\n" +
                        "---------------------------------------------\n" +
                        "Elija una opción\n" +
                        "*1*.Ver reportes sin revisar\n" +
                        "*2*.Crear proyecto\n" +
                        "*3*.Asignar tarea\n" +
                        "*4*.Tareas pendientes\n" +
                        "*5*.Tareas pendientes\n" +
                        "*6*.Crear tarea\n" +
                        "*7*.Mostrar colaboradores\n" +
                        $"Hay *{reportesPendientes.Count()}* reportes sin revisar";
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
                        case "4":
                            return "Elejiste la opción 4";
                        case "5":
                            return "Elejiste la opción 5";
                        case "6":
                            return "Elejiste la opción 6";
                        case "7":
                            return "Elejiste la opción 7";
                        default:
                            return "Opción no valida";
                    }
            }

            return "Opción no valida";
        }
    }
}
