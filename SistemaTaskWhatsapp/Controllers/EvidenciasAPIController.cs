using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using System.Threading.Tasks;


[ApiController]
[Route("api/evidencias")]
[AllowAnonymous]
public class EvidenciasApiController : ControllerBase
{
    private readonly IContenedorTrabajo _contenedorTrabajo;

    public EvidenciasApiController(IContenedorTrabajo contenedorTrabajo)
    {
        _contenedorTrabajo = contenedorTrabajo;
    }

    [HttpGet("{reporteId}")]
    public async Task<IActionResult> GetByReporte(int reporteId)
    {
        var lista = await _contenedorTrabajo.Evidencia
            .GetAllAsync(e => e.ReporteId == reporteId);

        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var resultado = lista.Select(e => new EvidenciaDto
        {
            Id = e.Id,
            Url = $"{baseUrl}/{e.Url}",
            Descripcion = e.Descripcion,
            ReporteId = e.ReporteId
        });

        return Ok(resultado);
    }

}
