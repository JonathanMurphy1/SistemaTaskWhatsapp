using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ProgramasApiController : ControllerBase
{
    private readonly IContenedorTrabajo _contenedorTrabajo;

    public ProgramasApiController(IContenedorTrabajo contenedorTrabajo)
    {
        _contenedorTrabajo = contenedorTrabajo;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var lista = await _contenedorTrabajo.Programa.GetAllAsync();
        return Ok(lista);
    }
}
