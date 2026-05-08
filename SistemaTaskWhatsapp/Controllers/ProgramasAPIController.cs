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

    [HttpPost("update")]
    public async Task<IActionResult> ActualizarPrograma([FromBody] ProgramaCreateDto dto)
    {
        if (dto == null || dto.Id == 0 || string.IsNullOrEmpty(dto.Nombre))
            return BadRequest();

        var programa = await _contenedorTrabajo.Programa
            .GetByIdAsync(dto.Id);

        if (programa == null)
            return NotFound(new { message = "Programa no encontrado" });

        int programaId = dto.Id;

        //Validación
        if (programa.Id != programaId)
        {
            return StatusCode(403, new { message = "No tienes permisos para modificar este programa" });
        }

        // Solo actualizar nombre
        programa.Nombre = dto.Nombre;

        _contenedorTrabajo.Programa.Update(programa);
        await _contenedorTrabajo.SaveAsync();

        return Ok(new
        {
            message = "Programa actualizado correctamente"
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarPrograma(int id, [FromQuery] int programaId)
    {
        var programa = await _contenedorTrabajo.Programa
            .GetByIdAsync(id);

        if (programa == null)
            return NotFound(new { message = "Programa no existe" });

        var tieneRelaciones = await _contenedorTrabajo.Empresa
                                    .GetFirstOrDefaultAsync(e => e.ProgramaOrigenId == id);

        if (tieneRelaciones != null)
        {
            return BadRequest(new
            {
                message = "No se puede eliminar el programa porque tiene registros asociados"
            });
        }

        //Validación
        if (programa.Id != programaId)
        {
            return StatusCode(403, new { message = "No tienes permisos para eliminar este programa" });
        }

        _contenedorTrabajo.Programa.Remove(programa);
        await _contenedorTrabajo.SaveAsync();

        return Ok(new
        {
            message = "Programa eliminado correctamente"
        });
    }
}
