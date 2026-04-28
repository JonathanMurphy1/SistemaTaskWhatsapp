using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Utilidades;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Controllers
{
    [ApiController]
    [Route("api/proyectos")]
    [AllowAnonymous]
    public class ProyectosApiController : ControllerBase
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public ProyectosApiController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }


        [HttpGet]
        public async Task<IActionResult> GetProyectos()
        {
            var lista = await _contenedorTrabajo.Proyecto.GetAllAsync();

            var resultado = lista.Select(p => new ProyectoResponseDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                FechaRegistro = p.FechaRegistro,
                FechaFin = p.FechaFin,
                EmpresaId = p.EmpresaId,
                ProgramaId = p.ProgramaId,
                Estado = (int)p.Estado,
                IdExterno = p.IdExterno
            });

            return Ok(resultado);
        }


        [HttpPost]
        public async Task<IActionResult> RecibirProyecto([FromBody] ProyectoCreateDto dto)
        {
            if (dto == null)
                return BadRequest();

            var proyecto = new Proyecto
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                FechaRegistro = dto.FechaRegistro ?? DateTime.Now,
                EmpresaId = dto.EmpresaId,
                ProgramaId = dto.ProgramaId,
                Estado = (EstadosProyecto)dto.Estado,
                IdExterno = dto.IdExterno
            };

            await _contenedorTrabajo.Proyecto.AddAsync(proyecto);
            await _contenedorTrabajo.SaveAsync();

            return Ok(new { message = "Proyecto creado correctamente" });
        }

        //Editar proyectos
        [HttpPost("update")]
        public async Task<IActionResult> ActualizarProyecto([FromBody] ProyectoCreateDto dto)
        {
            if (dto == null)
                return BadRequest();

            var proyecto = await _contenedorTrabajo.Proyecto
                .GetFirstOrDefaultAsync(p => p.IdExterno == dto.IdExterno);

            if (proyecto == null)
                return NotFound();

            proyecto.Nombre = dto.Nombre;
            proyecto.Descripcion = dto.Descripcion;
            proyecto.EmpresaId = dto.EmpresaId;
            proyecto.ProgramaId = dto.ProgramaId;
            proyecto.FechaRegistro = dto.FechaRegistro ?? proyecto.FechaRegistro;
            proyecto.Estado = (EstadosProyecto)dto.Estado;

            _contenedorTrabajo.Proyecto.Update(proyecto);
            await _contenedorTrabajo.SaveAsync();

            return Ok(new { message = "Proyecto actualizado correctamente" });
        }


        [HttpDelete("{idExterno}")]
        public async Task<IActionResult> EliminarProyecto(int idExterno)
        {
            var proyecto = await _contenedorTrabajo.Proyecto
                .GetFirstOrDefaultAsync(p => p.IdExterno == idExterno);

            if (proyecto == null)
                return NotFound(new { message = "Proyecto no existe" });

            _contenedorTrabajo.Proyecto.Remove(proyecto);
            await _contenedorTrabajo.SaveAsync();

            return Ok(new { message = "Proyecto eliminado correctamente" });
        }
    }
}