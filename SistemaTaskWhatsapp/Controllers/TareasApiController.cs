using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Utilidades;

namespace SistemaTaskWhatsapp.Controllers
{
    [ApiController]
    [Route("api/tareas")]
    [AllowAnonymous]
    public class TareasApiController : ControllerBase
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public TareasApiController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        [HttpGet]
        public async Task<IActionResult> GetTareas()
        {
            var lista = await _contenedorTrabajo.Tarea.GetAllAsync();

            var resultado = lista.Select(t => new TareaResponseDto
            {
                Id = t.Id,
                Nombre = t.Nombre,
                Descripcion = t.Descripcion,
                FechaInicio = t.FechaInicio,
                FechaTermino = t.FechaTermino,
                FechaEntrega = t.FechaEntrega,
                Estado = (int)t.Estado,
                ProyectoId = t.ProyectoId,
                IdExterno = t.IdExterno
            });

            return Ok(resultado);
        }

        [HttpPost]
        public async Task<IActionResult> RecibirTarea([FromBody] TareaCreateDto dto)
        {
            if (dto == null)
                return BadRequest();

            Proyecto proyecto = null;

            if (dto.ProyectoId.HasValue)
            {
                proyecto = await _contenedorTrabajo.Proyecto
                    .GetFirstOrDefaultAsync(p => p.Id == dto.ProyectoId);
            }

            if (proyecto == null)
                return BadRequest("Proyecto no encontrado");

            var tarea = new Tarea
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                FechaInicio = dto.FechaInicio ?? DateTime.Now,
                FechaEntrega = dto.FechaEntrega ?? DateTime.Now.AddDays(1),
                Estado = (EstadosTarea)dto.Estado,
                ProyectoId = proyecto.Id,
                IdExterno = dto.IdExterno
            };

            await _contenedorTrabajo.Tarea.AddAsync(tarea);
            await _contenedorTrabajo.SaveAsync();

            return Ok(new { message = "Tarea creada correctamente" });
        }

        [HttpPut]
        public async Task<IActionResult> ActualizarTarea([FromBody] TareaCreateDto dto, [FromQuery] int programaId)
        {
            if (dto == null || dto.IdExterno == null)
                return BadRequest();

            var tarea = await _contenedorTrabajo.Tarea
                .GetFirstOrDefaultAsync(t => t.IdExterno == dto.IdExterno, 
                                        includeProperties: "Proyecto");

            if (tarea == null)
                return NotFound($"No existe tarea con IdExterno {dto.IdExterno}");
            
            //Validación
            if (tarea.Proyecto.ProgramaId != programaId)
            {
                return Forbid("No tienes permisos para modificar esta tarea");
            }

            tarea.Nombre = dto.Nombre;
            tarea.Descripcion = dto.Descripcion;

            if (dto.Estado >= 0)
            {
                tarea.Estado = (EstadosTarea)dto.Estado;

                if (tarea.Estado == EstadosTarea.Finalizada)
                    tarea.FechaTermino = DateTime.Now;
                else
                    tarea.FechaTermino = null;
            }

            _contenedorTrabajo.Tarea.Update(tarea);
            await _contenedorTrabajo.SaveAsync();

            return Ok(new { message = "Tarea actualizada correctamente" });
        }

        [HttpDelete("{idExterno}")]
        public async Task<IActionResult> EliminarTarea(int idExterno, [FromQuery] int programaId)
        {
            var tarea = await _contenedorTrabajo.Tarea
                .GetFirstOrDefaultAsync(t => t.IdExterno == idExterno, 
                                        includeProperties: "Proyecto");

            if (tarea == null)
                return NotFound(new { message = "Tarea no existe" });

            //Validación
            if (tarea.Proyecto.ProgramaId != programaId)
            {
                return Forbid("No tienes permisos para eliminar esta tarea");
            }

            _contenedorTrabajo.Tarea.Remove(tarea);
            await _contenedorTrabajo.SaveAsync();

            return Ok(new { message = "Tarea eliminada correctamente" });
        }

        [HttpPost("asignar-responsable")]
        public async Task<IActionResult> AsignarResponsable([FromBody] AsignarResponsableDto dto, [FromQuery] int programaId)
        {
            if (dto == null)
                return BadRequest();

            //Buscar la tarea usando IdExterno
            var tarea = await _contenedorTrabajo.Tarea
                .GetFirstOrDefaultAsync(t => t.IdExterno == dto.IdExterno, 
                                        includeProperties: "Proyecto");

            if (tarea == null)
            {
                return BadRequest($"No existe tarea con IdExterno {dto.IdExterno}");
            }

            //Validación
            if (tarea.Proyecto.ProgramaId != programaId)
            {
                return Forbid("No tienes permisos para modificar esta tarea");
            }

            //Buscar usuario
            var usuario = await _contenedorTrabajo.Usuario
                .GetFirstOrDefaultAsync(u => u.IdExterno == dto.UserId);

            if (usuario == null)
            {
                return BadRequest($"No existe usuario {dto.UserId}");
            }

            //Buscar empleado en base al usuario
            var empleado = await _contenedorTrabajo.Empleado
                .GetFirstOrDefaultAsync(e => e.UsuarioId == usuario.Id);

            if (empleado == null)
            {
                return BadRequest($"No existe empleado para usuario {dto.UserId}");
            }

            //Eliminar responsables anteriores
            var responsablesActuales = await _contenedorTrabajo.TareaEmpleado
                .GetAllAsync(te => te.TareaId == tarea.Id);

            foreach (var rel in responsablesActuales)
            {
                _contenedorTrabajo.TareaEmpleado.Remove(rel);
            }

            var nuevo = new TareaEmpleado
            {
                TareaId = tarea.Id,
                EmpleadoId = empleado.Id
            };

            await _contenedorTrabajo.TareaEmpleado.AddAsync(nuevo);
            await _contenedorTrabajo.SaveAsync();

            return Ok(new { message = "Responsable asignado correctamente" });
        }
    }
}