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

    [HttpPost]
    public async Task<IActionResult> CrearPrograma([FromBody] ProgramaCreateDto dto)
    {
        if (dto == null || string.IsNullOrEmpty(dto.Nombre))
            return BadRequest();

        var programa = new Programa
        {
            Nombre = dto.Nombre,
            FechaRegistro = DateTime.Now
        };

        await _contenedorTrabajo.Programa.AddAsync(programa);
        await _contenedorTrabajo.SaveAsync();

        //Callback
        if (!string.IsNullOrEmpty(dto.CallbackUrl))
        {
            try
            {
                using var httpClient = new HttpClient();

                await httpClient.PostAsJsonAsync(dto.CallbackUrl, new
                {
                    id = programa.Id,
                    nombre = programa.Nombre
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en callback: {ex.Message}");
            }
        }

        return Ok(new
        {
            id = programa.Id,
            message = "Programa creado correctamente"
        });
    }
}
