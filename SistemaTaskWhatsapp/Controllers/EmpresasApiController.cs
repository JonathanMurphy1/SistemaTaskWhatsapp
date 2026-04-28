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
    }
}