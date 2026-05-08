using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using System.Threading.Tasks;

[ApiController]
[Route("api/reportes")]
[AllowAnonymous]
public class ReportesApiController : ControllerBase
{
    private readonly IContenedorTrabajo _contenedorTrabajo;

    public ReportesApiController(IContenedorTrabajo contenedorTrabajo)
    {
        _contenedorTrabajo = contenedorTrabajo;
    }

    [HttpGet]
    public async Task<IActionResult> GetReportes()
    {
        var lista = await _contenedorTrabajo.Reporte.GetAllAsync(
            includeProperties: "Tarea,Empleado,Empleado.Usuario"
        );

        var resultado = lista.Select(r => new ReporteResponseDto
        {
            Id = r.Id,
            Nombre = r.Nombre,
            Contenido = r.Contenido,
            Inconvenientes = r.Inconvenientes,
            ComentarioEmpleado = r.ComentarioEmpleado,
            Estado = r.Estado.ToString(),
            FechaSubida = r.FechaSubida,
            TareaId = r.TareaId,
            TareaNombre = r.Tarea != null ? r.Tarea.Nombre : "Sin tarea",
            EmpleadoId = r.EmpleadoId,
            EmpleadoNombre = r.Empleado != null && r.Empleado.Usuario != null
                ? r.Empleado.Usuario.Nombre
                : "Sin asignar"
        });

        return Ok(resultado);
    }
}
