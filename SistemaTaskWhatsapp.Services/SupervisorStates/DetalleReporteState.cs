using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Models.ViewModels;
using SistemaTaskWhatsapp.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Services.SupervisorStates
{
    internal class DetalleReporteState : ISupervisorState
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public DetalleReporteState(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }
        public async Task<string> HandleAsync(Usuario usuario, ChatSession sesion, string mensaje)
        {
            string mensajeLower = mensaje.ToLower();
            string respuesta = "";
            sesion.DatosParciales = "";
            if (mensajeLower == "inicio")
            {
                return "Volviendo al inicio";
            }

            if (!int.TryParse(mensaje, out int reporteId))
            {
                return "Ingrese un valor valido"; // o maneja el error
            }

            var reporte = await _contenedorTrabajo.Reporte
                .GetFirstOrDefaultAsync(r => r.Id == reporteId && r.Estado == EstadosReporte.PendienteRevisar
                , includeProperties: "Empleado,Tarea.Proyecto");

            if (reporte == null)
            {
                return "No se encontro el reporte, elija uno valido";
            }

            respuesta = $"Id: {reporte.Id}\n" +
                $"Titulo: {reporte.Nombre}\n" +
                $"Tarea: {reporte.Tarea.Nombre}\n" +
                $"Proyecto: {reporte.Tarea.Proyecto.Nombre}\n" +
                $"Colaborador: {reporte.Empleado.Nombre}\n" +
                $"Contenido: {reporte.Contenido}\n" +
                $"Inconvenientes: {reporte.Inconvenientes ?? "Sin inconvenientes"}\n" +
                $"Comentarios: {reporte.ComentarioEmpleado ?? "Sin comentarios"}\n" +
                $"Fecha de subida: {reporte.FechaSubida.ToString("dd/MM/yyyy")}\n" +
                $"-------------------------------------------------------------------\n" +
                $"¿Quieres dar retroalimentación de esta tarea?\n" +
                $"Escribe *Si* si quieres hacerlo, o cualquier cosa para revisar otros reportes";

            sesion.EstadoStep = "DarRetroalimentacion";
            var data = SessionJsonHelper.GetData<RevisarVM>(sesion.DatosParciales);
            data.TareaId = (int)reporte.TareaId;
            data.ReporteId = reporte.Id;
            sesion.DatosParciales = SessionJsonHelper.SetData(data);
            return respuesta;
        }
    }
}
