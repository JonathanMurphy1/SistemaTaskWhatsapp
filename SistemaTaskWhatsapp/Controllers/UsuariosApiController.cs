using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Utilidades;
using System.Threading.Tasks;
using SistemaTaskWhatsapp.Utilidades;

namespace SistemaTaskWhatsapp.Controllers
{
    [ApiController]
    [Route("api/users")]
    [AllowAnonymous]
    public class UsuariosApiController : ControllerBase
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly UserManager<Usuario> _userManager;

        public UsuariosApiController(UserManager<Usuario> userManager, IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> CrearUsuario([FromBody] UsuarioDto dto)
        {
            if (dto == null)
                return BadRequest();

            var existe = await _contenedorTrabajo.Usuario
                .GetFirstOrDefaultAsync(u => u.UserId == dto.UserId || u.Email == dto.Email);

            if (existe != null)
            {
                return Ok(new { message = "El usuario ya existe" });
            }

            var usuario = new Usuario
            {
                UserId = dto.UserId,
                Nombre = dto.Name,
                UserName = dto.Email,
                Email = dto.Email,
                PhoneNumber = null,
                Rol = RoleMapper.MapFromLaravel(dto.UserTypeId)
            };

            var resultado = await _userManager.CreateAsync(usuario, dto.Password);

            if (!resultado.Succeeded)
            {
                return BadRequest(resultado.Errors);
            }

            await _userManager.AddToRoleAsync(usuario, usuario.Rol.ToString());

            if (usuario.Rol == Roles.Supervisor)
            {
                await _contenedorTrabajo.Supervisor.AddAsync(new Supervisor
                {
                    Nombre = usuario.Nombre,
                    UsuarioId = usuario.Id,
                    Estado = EstadosSupervisor.Activo
                });
            }
            else if (usuario.Rol == Roles.Empleado)
            {
                await _contenedorTrabajo.Empleado.AddAsync(new Empleado
                {
                    Nombre = usuario.Nombre,
                    UsuarioId = usuario.Id,
                    FechaRegistro = DateTime.Now,
                    Estado = EstadosEmpleado.Activo
                });
            }

            await _contenedorTrabajo.SaveAsync();

            return Ok(new { message = "Usuario creado correctamente" });
        }

        public static class RoleMapper
        {
            public static Roles MapFromLaravel(int userTypeId)
            {
                return userTypeId switch
                {
                    1 => Roles.Supervisor,     // Administrador -> Supervisor
                    2 => Roles.Empleado,       // Practicante -> Empleado
                    4 => Roles.Administrador,  // Super Admin -> Administrador
                    40 => Roles.Empleado,      // JCF -> Empleado
                    _ => Roles.Empleado        // default por seguridad
                };
            }
        }
    }
}