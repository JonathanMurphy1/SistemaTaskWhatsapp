using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using System.Threading.Tasks;

[Route("api/programas")]
[ApiController]
public class ProgramasApiController : ControllerBase
{
    private readonly IContenedorTrabajo _contenedorTrabajo;

    public ProgramasApiController(IContenedorTrabajo contenedorTrabajo)
    {
        _contenedorTrabajo = contenedorTrabajo;
    }

    //Mostrar datos en un Json
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var lista = await _contenedorTrabajo.Programa.GetAllAsync();

        var resultado = lista.Select(p => new ProgramaResponseDto
        {
            Id = p.Id,
            Nombre = p.Nombre,
            FechaRegistro = p.FechaRegistro
        });

        return Ok(resultado);
    }
}
