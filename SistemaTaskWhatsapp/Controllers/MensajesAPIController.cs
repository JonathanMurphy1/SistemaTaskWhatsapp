using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using System.Threading.Tasks;

[Route("api/mensajes")]
[ApiController]
public class MensajesApiController : ControllerBase
{
    private readonly IContenedorTrabajo _contenedorTrabajo;

    public MensajesApiController(IContenedorTrabajo contenedorTrabajo)
    {
        _contenedorTrabajo = contenedorTrabajo;
    }

    //Mostrar datos en un Json
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var lista = await _contenedorTrabajo.Mensaje.GetAllAsync();

        var resultado = lista.Select(m => new MensajeResponseDto
        {
            Id = m.Id,
            Tipo = m.Tipo,
            Contenido = m.Contenido,
            Cron = m.Cron,
            Activo = m.Activo, 
            FechaCreacion = m.FechaCreacion,
            EmpresaId = (int)m.EmpresaId
        });

        return Ok(resultado);
    }
}
