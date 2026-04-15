using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Controllers
{
    [ApiController]
    [Route("api/companies")]
    [AllowAnonymous]
    public class CompaniesApiController : ControllerBase
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public CompaniesApiController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        [HttpPost]
        public async Task<IActionResult> CrearEmpresa([FromBody] EmpresaDto dto)
        {
            if (dto == null)
                return BadRequest();

            //Validacion de Id
            var existe = await _contenedorTrabajo.Empresa
                .GetFirstOrDefaultAsync(e => e.CompaniesId == dto.CompaniesId);

            if (existe != null)
            {
                return Ok(new { message = "Empresa ya sincronizada" });
            }

            var empresa = new Empresa
            {
                CompaniesId = dto.CompaniesId,
                Nombre = dto.Name,
                FechaRegistro = DateTime.Now
            };

            await _contenedorTrabajo.Empresa.AddAsync(empresa);
            await _contenedorTrabajo.SaveAsync();

            return Ok(new { message = "Empresa creada correctamente" });
        }
    }
}