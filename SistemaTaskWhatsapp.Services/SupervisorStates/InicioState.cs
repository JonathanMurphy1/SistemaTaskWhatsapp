using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Services.SupervisorStates
{
    internal class InicioState : ISupervisorState
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public InicioState(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }
        public async Task<string> HandleAsync(Usuario usuario, ChatSession sesion, string mensaje)
        {
            sesion.DatosParciales = "";
            sesion.EstadoStep = "Menu";

            var reportesPendientes = await _contenedorTrabajo.Reporte.GetAllAsync(r => r.Estado == EstadosReporte.PendienteRevisar);

            string respuesta = $"Hola Supervisor {usuario.Nombre}\n" +
                "---------------------------------------------\n" +
                "Elija una opción\n" +
                "*1*.Ver reportes sin revisar\n" +
                "*2*.Crear proyecto\n" +
                "*3*.Asignar tarea\n" +
                "*4*.Proyectos pendientes\n" +
                "*5*.Tareas pendientes\n" +
                "*6*.Crear tarea\n" +
                "*7*.Mostrar colaboradores\n" +
                "------------------------------------------------\n" +
                $"Hay *{reportesPendientes.Count()}* reportes pendientes de revisar\n" +
                $"Pedes escribir *Inicio* en cualquier momento para volver a este menú";

            return respuesta;
        }
    }
}
