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
                ProyectoId = proyecto.Id
            };

            await _contenedorTrabajo.Tarea.AddAsync(tarea);
            await _contenedorTrabajo.SaveAsync();

            return Ok(new { message = "Tarea recibida correctamente" });
        }
    }
}