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


        //Modificar el usuario desde perfil y tabla
        [HttpPost("update")]
        public async Task<IActionResult> ActualizarUsuario([FromBody] UsuarioUpdateDto dto)
        {
            if (dto == null)
                return BadRequest();

            var usuario = await _contenedorTrabajo.Usuario
                .GetFirstOrDefaultAsync(u => u.UserId == dto.UserId);

            if (usuario == null)
                return NotFound();

            usuario.Nombre = dto.Name;
            usuario.Email = dto.Email;
            usuario.UserName = dto.Email;
            
            //El numero de telefono se vuelve opcional ya que en tabla 
            //de usuario no se actualiza
            if (dto.PhoneNumber != null)
            {
                usuario.PhoneNumber = dto.PhoneNumber;
            }


            ///Proceso para cambiar el rol del usuario en TasWhatsApp
            Roles? nuevoRol = null;

            if (dto.UserTypeId.HasValue)
            {
                nuevoRol = RoleMapper.MapFromLaravel(dto.UserTypeId.Value);

                var rolesActuales = await _userManager.GetRolesAsync(usuario);
                await _userManager.RemoveFromRolesAsync(usuario, rolesActuales);
                await _userManager.AddToRoleAsync(usuario, nuevoRol.ToString());

                usuario.Rol = nuevoRol.Value;
            }

            var resultado = await _userManager.UpdateAsync(usuario);

            if (!resultado.Succeeded)
                return BadRequest(resultado.Errors);

            //Se hace al cambio si fue necesario
            if (nuevoRol.HasValue)
            {
                // Supervisor
                if (nuevoRol == Roles.Supervisor)
                {
                    var supervisor = await _contenedorTrabajo.Supervisor
                        .GetFirstOrDefaultAsync(s => s.UsuarioId == usuario.Id);

                    if (supervisor == null)
                    {
                        await _contenedorTrabajo.Supervisor.AddAsync(new Supervisor
                        {
                            UsuarioId = usuario.Id,
                            Nombre = usuario.Nombre,
                            Estado = EstadosSupervisor.Activo
                        });
                    }
                    else
                    {
                        supervisor.Nombre = usuario.Nombre;
                        _contenedorTrabajo.Supervisor.Update(supervisor);
                    }

                    //Eliminar empleado si existía
                    var empleado = await _contenedorTrabajo.Empleado
                        .GetFirstOrDefaultAsync(e => e.UsuarioId == usuario.Id);

                    if (empleado != null)
                        _contenedorTrabajo.Empleado.Remove(empleado);
                }

                //Empleado
                else if (nuevoRol == Roles.Empleado)
                {
                    var empleado = await _contenedorTrabajo.Empleado
                        .GetFirstOrDefaultAsync(e => e.UsuarioId == usuario.Id);

                    if (empleado == null)
                    {
                        await _contenedorTrabajo.Empleado.AddAsync(new Empleado
                        {
                            UsuarioId = usuario.Id,
                            Nombre = usuario.Nombre,
                            Estado = EstadosEmpleado.Activo,
                            FechaRegistro = DateTime.Now
                        });
                    }
                    else
                    {
                        empleado.Nombre = usuario.Nombre;
                        _contenedorTrabajo.Empleado.Update(empleado);
                    }

                    //Eliminar supervisor si existía
                    var supervisor = await _contenedorTrabajo.Supervisor
                        .GetFirstOrDefaultAsync(s => s.UsuarioId == usuario.Id);

                    if (supervisor != null)
                        _contenedorTrabajo.Supervisor.Remove(supervisor);
                }

                //Administrador
                else
                {
                    var supervisor = await _contenedorTrabajo.Supervisor
                        .GetFirstOrDefaultAsync(s => s.UsuarioId == usuario.Id);

                    if (supervisor != null)
                        _contenedorTrabajo.Supervisor.Remove(supervisor);

                    var empleado = await _contenedorTrabajo.Empleado
                        .GetFirstOrDefaultAsync(e => e.UsuarioId == usuario.Id);

                    if (empleado != null)
                        _contenedorTrabajo.Empleado.Remove(empleado);
                }
            }

            await _contenedorTrabajo.SaveAsync();

            return Ok(new { message = "Usuario actualizado correctamente" });
       
        }

        [HttpPost("delete")]
        public async Task<IActionResult> EliminarUsuario([FromBody] UsuarioDeleteDto dto)
        {
            if (dto == null)
                return BadRequest();

            var usuario = await _contenedorTrabajo.Usuario
                .GetFirstOrDefaultAsync(u => u.UserId == dto.UserId);

            if (usuario == null)
                return Ok(new { message = "Usuario no existe" });

            //Eliminar Supervisor si existe
            var supervisor = await _contenedorTrabajo.Supervisor
                .GetFirstOrDefaultAsync(s => s.UsuarioId == usuario.Id);

            if (supervisor != null)
                _contenedorTrabajo.Supervisor.Remove(supervisor);

            //Eliminar Empleado si existe
            var empleado = await _contenedorTrabajo.Empleado
                .GetFirstOrDefaultAsync(e => e.UsuarioId == usuario.Id);

            if (empleado != null)
                _contenedorTrabajo.Empleado.Remove(empleado);

            await _contenedorTrabajo.SaveAsync();

            //Eliminar con Identity
            var resultado = await _userManager.DeleteAsync(usuario);

            if (!resultado.Succeeded)
                return BadRequest(resultado.Errors);

            return Ok(new { message = "Usuario eliminado correctamente" });
        }

        //Función para determinar el equivalente del rol en TaskWhatsApp
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