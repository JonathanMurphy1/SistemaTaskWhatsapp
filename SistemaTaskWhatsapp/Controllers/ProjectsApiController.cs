using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Utilidades;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Controllers
{
    [ApiController]
    [Route("api/projects")]
    [AllowAnonymous]
    public class ProjectsApiController : ControllerBase
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public ProjectsApiController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }


        [HttpGet]
        public async Task<IActionResult> GetProyectos()
        {
            var lista = await _contenedorTrabajo.Proyecto.GetAllAsync();

            var resultado = lista.Select(p => new ProjectDto
            {
                Idproject = p.Id,
                Name = p.Nombre,
                Description = p.Descripcion,
                StartDate = p.FechaRegistro,
                EmpresaId = p.EmpresaId,
                Estado = (int)p.Estado,
               
            });

            return Ok(resultado);
        }


        [HttpPost]
        public async Task<IActionResult> RecibirProyecto([FromBody] ProjectDto dto)
        {
            if (dto == null)
                return BadRequest();

            var proyecto = new Proyecto
            {

                Nombre = dto.Name,
                Descripcion = dto.Description,
                FechaRegistro = dto.StartDate ?? DateTime.Now,
                EmpresaId = dto.EmpresaId,
                Estado = (EstadosProyecto)dto.Estado

            };

            await _contenedorTrabajo.Proyecto.AddAsync(proyecto);
            await _contenedorTrabajo.SaveAsync();

            return Ok(new { message = "Proyecto recibido correctamente" });
        }

        //Editar proyectos
        [HttpPost("update")]
        public async Task<IActionResult> ActualizarProyecto([FromBody] ProjectDto dto)
        {
            if (dto == null)
                return BadRequest();

            var proyecto = await _contenedorTrabajo.Proyecto
                .GetFirstOrDefaultAsync(p => p.IdExterno == dto.Idproject);

            if (proyecto == null)
                return NotFound();

            proyecto.Nombre = dto.Name;
            proyecto.Descripcion = dto.Description;
            proyecto.EmpresaId = dto.EmpresaId;
            proyecto.FechaRegistro = dto.StartDate ?? DateTime.Now;
            proyecto.Estado = (EstadosProyecto)dto.Estado;

            _contenedorTrabajo.Proyecto.Update(proyecto);
            await _contenedorTrabajo.SaveAsync();

            return Ok(new { message = "Proyecto actualizado correctamente" });
        }

        [HttpDelete("{Idproject}")]
        public async Task<IActionResult> EliminarProyecto(int Idproject)
        {
            var proyecto = await _contenedorTrabajo.Proyecto
                .GetFirstOrDefaultAsync(p => p.IdExterno == Idproject);

            if (proyecto == null)
                return NotFound(new { message = "Proyecto no existe" });

            _contenedorTrabajo.Proyecto.Remove(proyecto);
            await _contenedorTrabajo.SaveAsync();

            return Ok(new { message = "Proyecto eliminado correctamente" });
        }
    }
}