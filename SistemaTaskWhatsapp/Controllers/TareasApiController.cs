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

        [HttpPost]
        public async Task<IActionResult> RecibirTarea([FromBody] TareaDto dto)
        {
            if (dto == null)
                return BadRequest();

            //Se busca el proyecto por nombre
            var proyecto = await _contenedorTrabajo.Proyecto
                    .GetFirstOrDefaultAsync(p =>
                     p.Nombre.Trim().ToLower() == dto.ProjectName.Trim().ToLower()); //Se ajusta el formato para que coicida con Task

            if (proyecto == null)
            {
                Console.WriteLine("Proyecto no encontrado: " + dto.ProjectName);
                return BadRequest($"Proyecto no encontrado: {dto.ProjectName}");
            }

            var tarea = new Tarea
            {
                Nombre = dto.Title,
                Descripcion = dto.Description,
                FechaInicio = dto.StartDate ?? DateTime.Now,
                FechaEntrega = dto.DeliveryDate ?? DateTime.Now.AddDays(1),
                Estado = (EstadosTarea)dto.Estado,
                ProyectoId = proyecto.Id,
                SubtaskId = dto.SubtaskId
            };

            await _contenedorTrabajo.Tarea.AddAsync(tarea);
            await _contenedorTrabajo.SaveAsync();

            return Ok(new { message = "Tarea recibida correctamente" });
        }

        [HttpPut]
        public async Task<IActionResult> ActualizarTarea([FromBody] TareaDto dto)
        {
            var tarea = await _contenedorTrabajo.Tarea
                .GetFirstOrDefaultAsync(t => t.SubtaskId == dto.SubtaskId);

            if (tarea == null)
                return NotFound($"No existe tarea con SubtaskId {dto.SubtaskId}");

            //Unicamente se actualizan estos datos en task
            tarea.Nombre = dto.Title;
            tarea.Descripcion = dto.Description;

            _contenedorTrabajo.Tarea.Update(tarea);
            await _contenedorTrabajo.SaveAsync();

            return Ok(new { message = "Tarea actualizada correctamente" });
        }
    }
}