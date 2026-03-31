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

        [HttpPost]
        public async Task<IActionResult> RecibirProyecto([FromBody] ProjectDto dto)
        {
            if (dto == null)
                return BadRequest();

            var proyecto = new Proyecto
            {
                ProjectId = dto.Idproject,
                Nombre = dto.Name,
                Descripcion = dto.Description,
                FechaRegistro = DateTime.Now,
                EmpresaId = dto.EmpresaId,
                Estado = (EstadosProyecto)dto.Estado

            };

            await _contenedorTrabajo.Proyecto.AddAsync(proyecto);
            await _contenedorTrabajo.SaveAsync();

            return Ok(new { message = "Proyecto recibido correctamente" });
        }
    }
}