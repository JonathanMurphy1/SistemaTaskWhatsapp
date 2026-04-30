using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Controllers
{
    [ApiController]
    [Route("api/empresas")]
    [AllowAnonymous]
    public class EmpresasApiController : ControllerBase
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public EmpresasApiController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        //Mostrar datos en un Json
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var lista = await _contenedorTrabajo.Empresa.GetAllAsync();

            var resultado = lista.Select(e => new EmpresaResponseDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                FechaRegistro = e.FechaRegistro,
                IdExterno = e.IdExterno,
                ProgramaOrigenId = e.ProgramaOrigenId
            });

            return Ok(resultado);
        }

        [HttpPost]
        public async Task<IActionResult> CrearEmpresa([FromBody] EmpresaCreateDto dto)
        {
            if (dto == null)
                return BadRequest();

            var existe = await _contenedorTrabajo.Empresa
                .GetFirstOrDefaultAsync(e => e.IdExterno == dto.IdExterno);

            if (existe != null)
            {
                return Ok(new { message = "Empresa ya sincronizada" });
            }

            var empresa = new Empresa
            {
                Nombre = dto.Nombre,
                IdExterno = dto.IdExterno,
                ProgramaOrigenId = dto.ProgramaOrigenId,
                FechaRegistro = DateTime.Now
            };

            await _contenedorTrabajo.Empresa.AddAsync(empresa);
            await _contenedorTrabajo.SaveAsync();

            return Ok(new { message = "Empresa creada correctamente" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditarEmpresa(int id, [FromBody] EmpresaCreateDto dto)
        {
            if (dto == null)
                return BadRequest("Datos inválidos");

            var empresa = await _contenedorTrabajo.Empresa.GetByIdAsync(id);

            if (empresa == null)
                return NotFound("Empresa no encontrada");

            //Validación de permisos
            if (empresa.ProgramaOrigenId != dto.ProgramaOrigenId)
            {
                return Forbid("No tienes permisos para editar esta empresa");
            }

            //Validar duplicado
            var existe = await _contenedorTrabajo.Empresa
                .GetFirstOrDefaultAsync(e =>
                    e.Nombre == dto.Nombre && e.Id != id);

            if (existe != null)
            {
                return BadRequest("Ya existe una empresa con ese nombre");
            }

            // Actualizar datos
            empresa.Nombre = dto.Nombre;
            //empresa.IdExterno = dto.IdExterno;

            await _contenedorTrabajo.SaveAsync();

            return Ok(new { message = "Empresa actualizada correctamente" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarEmpresa(int id, [FromQuery] int programaId)
        {
            var empresa = await _contenedorTrabajo.Empresa.GetByIdAsync(id);

            if (empresa == null)
                return NotFound("Empresa no encontrada");

            //Validación de permisos
            if (empresa.ProgramaOrigenId != programaId)
            {
                return Forbid("No tienes permisos para eliminar esta empresa");
            }

            //Validar dependencias
            var tieneMensajes = await _contenedorTrabajo.Mensaje
                .GetFirstOrDefaultAsync(m => m.EmpresaId == id);

            if (tieneMensajes != null)
            {
                return BadRequest("No se puede eliminar la empresa porque tiene mensajes asociados");
            }

            //Eliminar relaciones primero
            var relaciones = await _contenedorTrabajo.EmpresaPrograma
                .GetAllAsync(x => x.EmpresaId == id);

            foreach (var rel in relaciones)
            {
                _contenedorTrabajo.EmpresaPrograma.Remove(rel);
            }

            //Eliminar empresa
            _contenedorTrabajo.Empresa.Remove(empresa);

            await _contenedorTrabajo.SaveAsync();

            return Ok(new { message = "Empresa eliminada correctamente" });
        }

    }
}