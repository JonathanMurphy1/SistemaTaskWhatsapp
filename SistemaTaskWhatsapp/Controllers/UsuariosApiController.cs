using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Models.ViewModels;
using SistemaTaskWhatsapp.Utilidades;
using SistemaTaskWhatsapp.Utilidades;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
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

        [HttpGet]
        public async Task<IActionResult> GetUsuarios()
        {
            var lista = await _contenedorTrabajo.Usuario
                .GetAllAsync(includeProperties: "Empresa,Programa");

            var resultado = lista.Select(u => new UsuarioResponseDto
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Rol = u.Rol.ToString(),
                EmpresaId = u.EmpresaId,
                EmpresaNombre = u.Empresa != null ? u.Empresa.Nombre : "Sin empresa",
                ProgramaId = (int)u.ProgramaId,
                ProgramaNombre = u.Programa != null ? u.Programa.Nombre : "Sin programa",
                IdExterno = u.IdExterno
            });

            return Ok(resultado);
        }

        [HttpPost]
        public async Task<IActionResult> CrearUsuario([FromBody] UsuarioCreateDto dto)
        {
            if (dto == null)
                return BadRequest();

            var existeEmail = await _userManager.FindByEmailAsync(dto.Email);
            if (existeEmail != null)
                return BadRequest("El email ya está registrado");

            //Validar IdExterno
            var existeExterno = await _contenedorTrabajo.Usuario
                .GetFirstOrDefaultAsync(u => u.IdExterno == dto.IdExterno);

            if (existeExterno != null)
                return Ok(new { message = "Usuario ya sincronizado" });


            var usuario = new Usuario
            {
                IdExterno = dto.IdExterno,
                Nombre = dto.Nombre,
                UserName = dto.Email,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                EmpresaId = dto.EmpresaId,
                ProgramaId = dto.ProgramaId,
                Rol = RoleMapper.MapFromLaravel(dto.UserTypeId)
            };

            //Crear usuario en Identity
            var resultado = await _userManager.CreateAsync(usuario, dto.Password);

            if (!resultado.Succeeded)
                return BadRequest(resultado.Errors);

            //Asignar rol
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
        public async Task<IActionResult> ActualizarUsuario([FromBody] UsuarioUpdateDto dto, [FromQuery] int programaId)
        {
            if (dto == null)
                return BadRequest();

            var usuario = await _contenedorTrabajo.Usuario
                .GetFirstOrDefaultAsync(u => u.IdExterno == dto.IdExterno);

            if (usuario == null)
                return NotFound();

            //Validación
            if (usuario.ProgramaId != dto.ProgramaId)
            {
                return StatusCode(403, new { message = "No tienes permisos para modificar este usuario" });
            }

            usuario.Nombre = dto.Nombre;
            usuario.Email = dto.Email;
            usuario.UserName = dto.Email;
            usuario.PhoneNumber = dto.PhoneNumber;
            

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
        public async Task<IActionResult> EliminarUsuario(int IdExterno, [FromQuery] int programaId)
        {
            var usuario = await _contenedorTrabajo.Usuario
                .GetFirstOrDefaultAsync(u => u.IdExterno == IdExterno);

            if (usuario == null)
                return Ok(new { message = "Usuario no existe" });

            if (usuario.ProgramaId == null || usuario.ProgramaId != programaId)
            {
                return StatusCode(403, new { message = "No tienes permisos para eliminar este usuario" });
            }

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
                    0 => Roles.Administrador,  // Origen a Administrador
                    1 => Roles.Supervisor,     // Origen a Supervisor
                    2 => Roles.Empleado,       // Origen a Empleado
                    _ => Roles.Empleado        //Cualquier otra opción será empleado por defecto
                };
            }
        }
    }
}