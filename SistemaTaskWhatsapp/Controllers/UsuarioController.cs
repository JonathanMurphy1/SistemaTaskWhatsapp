using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Data;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Models.ViewModels;
using SistemaTaskWhatsapp.Utilidades;

namespace SistemaTaskWhatsapp.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuarioController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsuarioController(UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager, IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var listaUsuarios = await _contenedorTrabajo.Usuario.GetAllAsync(u => u.Email != "admin@sistema.com", includeProperties: "Empresa,Programa");
            return View(listaUsuarios);
        }

        // GET: UsuarioController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UsuarioController/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new UsuarioCreateVM
            {
                Supervisor = new Supervisor(),
                Empleado = new Empleado(),
                ListaEmpresas = await _contenedorTrabajo.Empresa.GetEmpresasDropdown(),
                ListaProgramas = await _contenedorTrabajo.Programa.GetProgramaDropdown()
            };

            return View(model);
        }

        // POST: UsuarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioCreateVM model)
        {
            // Limpia validaciones que no aplican según el rol
            if (model.Rol != Roles.Supervisor)
            {
                ModelState.Remove("Supervisor.Estado");
            }

            if (model.Rol != Roles.Empleado)
            {
                ModelState.Remove("Empleado.Estado");
            }

            if (model.EmpresaId == null)
            {
                ModelState.AddModelError("EmpresaId", "Seleccione una empresa");
            }

            await ValidarDuplicidadCampos(model.Email, model.Telefono);

            if (!ModelState.IsValid)
            {
                model.ListaEmpresas = await _contenedorTrabajo.Empresa.GetEmpresasDropdown();
                model.ListaProgramas = await _contenedorTrabajo.Programa.GetProgramaDropdown();
                return View(model);
            }

            var empresa = await _contenedorTrabajo.Empresa
                                .GetByIdAsync(model.EmpresaId.Value);

            if (empresa == null)
            {
                ModelState.AddModelError("", "Empresa inválida");

                model.ListaEmpresas = await _contenedorTrabajo.Empresa.GetEmpresasDropdown();
                model.ListaProgramas = await _contenedorTrabajo.Programa.GetProgramaDropdown();
                return View(model);
            }


            var relacionValida = await _contenedorTrabajo.EmpresaPrograma
                                                            .GetFirstOrDefaultAsync(ep =>
                                                                ep.EmpresaId == model.EmpresaId &&
                                                                ep.ProgramaId == model.ProgramaId);

            if (relacionValida == null)
            {
                ModelState.AddModelError("", "La empresa no pertenece al programa seleccionado");

                model.ListaProgramas = await _contenedorTrabajo.Programa.GetProgramaDropdown();
                return View(model);
            }

            var usuario = new Usuario
            {
                Nombre = model.Nombre,
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.Telefono,
                Rol = model.Rol,
                EmpresaId = model.EmpresaId,
                ProgramaId = model.ProgramaId.Value
            };

            var resultado = await _userManager.CreateAsync(usuario, model.Password);

            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }

            //Ajustar el rol
            await _userManager.AddToRoleAsync(
                usuario,
                model.Rol.ToString()
            );

            if (model.Rol == Roles.Supervisor)
            {
                var nuevoSupervisor = new Supervisor
                {
                    Nombre = model.Nombre,
                    UsuarioId = usuario.Id,
                    Estado = model.Supervisor.Estado
                };

                await _contenedorTrabajo.Supervisor.AddAsync(nuevoSupervisor);
            }

            if (model.Rol == Roles.Empleado)
            {
                var nuevoEmpleado = new Empleado
                {
                    Nombre = model.Nombre,
                    UsuarioId = usuario.Id,
                    FechaRegistro = DateTime.Now,
                    Estado = model.Empleado.Estado
                };

                await _contenedorTrabajo.Empleado.AddAsync(nuevoEmpleado);
            }

            await _contenedorTrabajo.SaveAsync();

            TempData["Mensaje"] = $"Usuario con el nombre: {usuario.Nombre} creado correctamente";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
                return RedirectToAction("Index");

            var model = new EditarUsuarioVM
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Password = null,
                Telefono = usuario.PhoneNumber,
                Rol = usuario.Rol,
                EmpresaId = usuario.EmpresaId,
                ProgramaId = usuario.ProgramaId,
                ListaProgramas = await _contenedorTrabajo.Programa.GetProgramaDropdown(),
                Supervisor = new Supervisor(),
                Empleado = new Empleado()
            };

            if (model.ProgramaId.HasValue)
            {
                model.ListaEmpresas = await _contenedorTrabajo.Empresa
                    .GetEmpresasPorProgramaDropdown(model.ProgramaId.Value);
            }
            else
            {
                model.ListaEmpresas = new List<SelectListItem>();
            }

            if (usuario.Rol == Roles.Supervisor)
            {
                model.Supervisor = await _contenedorTrabajo.Supervisor
                    .GetFirstOrDefaultAsync(s => s.UsuarioId == usuario.Id)
                    ?? new Supervisor();
            }

            if (usuario.Rol == Roles.Empleado)
            {
                model.Empleado = await _contenedorTrabajo.Empleado
                    .GetFirstOrDefaultAsync(e => e.UsuarioId == usuario.Id)
                    ?? new Empleado();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditarUsuarioVM model)
        {
            //validaciones
            model.Supervisor ??= new Supervisor();
            model.Empleado ??= new Empleado();

            if (model.Rol != Roles.Supervisor)
            {
                ModelState.Remove("Supervisor.Estado");
            }

            if (model.Rol != Roles.Empleado)
            {
                ModelState.Remove("Empleado.Estado");
            }

            if (!ModelState.IsValid)
            {
                model.ListaEmpresas = await _contenedorTrabajo.Empresa.GetEmpresasDropdown();
                model.ListaProgramas = await _contenedorTrabajo.Programa.GetProgramaDropdown();

                return View(model);
            }

            await ValidarDuplicidadCampos(model.Email, model.Telefono, model.Id);

            if (!ModelState.IsValid)
            {
                model.ListaEmpresas = await _contenedorTrabajo.Empresa.GetEmpresasDropdown();

                model.ListaProgramas = await _contenedorTrabajo.Programa.GetProgramaDropdown();
                return View(model);
            }

            var usuario = await _userManager.FindByIdAsync(model.Id);
            if (usuario == null)
            {
                TempData["Mensaje"] = "Usuario no encontrado";
                TempData["Error"] = "Error";
                return RedirectToAction("Index");
            }

            var rolesActuales = await _userManager.GetRolesAsync(usuario);
            await _userManager.RemoveFromRolesAsync(usuario, rolesActuales);

            await _userManager.AddToRoleAsync(
                usuario,
                model.Rol.ToString()
            );

            //Obtener empresa
            var empresa = await _contenedorTrabajo.Empresa
                .GetByIdAsync(model.EmpresaId.Value);

            if (empresa == null)
            {
                ModelState.AddModelError("", "Empresa inválida");
                model.ListaEmpresas = await _contenedorTrabajo.Empresa.GetEmpresasDropdown();

                model.ListaProgramas = await _contenedorTrabajo.Programa.GetProgramaDropdown();
                return View(model);
            }

            //Usuario
            usuario.Nombre = model.Nombre;
            usuario.Email = model.Email;
            usuario.UserName = model.Email;
            usuario.PhoneNumber = model.Telefono;
            usuario.EmpresaId = model.EmpresaId;
            usuario.ProgramaId = model.ProgramaId;
            usuario.Rol = model.Rol;

            //Función por si se edita la contraseña
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);

                var passwordResult = await _userManager.ResetPasswordAsync(
                    usuario,
                    token,
                    model.Password
                );

                if (!passwordResult.Succeeded)
                {
                    foreach (var error in passwordResult.Errors)
                        ModelState.AddModelError("Password", error.Description);

                    return View(model);
                }
            }

            await _userManager.UpdateAsync(usuario);

            //Supervisor
            if (model.Rol == Roles.Supervisor)
            {
                var supervisor = await _contenedorTrabajo.Supervisor
                    .GetFirstOrDefaultAsync(s => s.UsuarioId == usuario.Id);

                if (supervisor == null)
                {
                    //Crea al supervisor si no existe al actualizar el rol
                    supervisor = new Supervisor
                    {
                        UsuarioId = usuario.Id,
                        Nombre = usuario.Nombre,
                        Estado = model.Supervisor.Estado
                    };

                    await _contenedorTrabajo.Supervisor.AddAsync(supervisor);
                }
                else
                {
                    //Actualiza al supervisor si ya existe
                    supervisor.Nombre = model.Nombre;
                    supervisor.Estado = model.Supervisor.Estado;

                    _contenedorTrabajo.Supervisor.Update(supervisor);
                }
            }
            else
            {
                //Borrar al supervisor si se cambia de rol
                var supervisor = await _contenedorTrabajo.Supervisor
                    .GetFirstOrDefaultAsync(s => s.UsuarioId == usuario.Id);

                if (supervisor != null)
                    _contenedorTrabajo.Supervisor.Remove(supervisor);
            }

            //Empleado
            if (model.Rol == Roles.Empleado)
            {
                var empleado = await _contenedorTrabajo.Empleado
                    .GetFirstOrDefaultAsync(e => e.UsuarioId == usuario.Id);

                if (empleado == null)
                {
                    //Crea al empleado si no existe al actualizar el rol
                    empleado = new Empleado
                    {
                        UsuarioId = usuario.Id,
                        Nombre = usuario.Nombre,
                        Estado = model.Empleado.Estado,
                        FechaRegistro = DateTime.Now
                    };

                    await _contenedorTrabajo.Empleado.AddAsync(empleado);
                }
                else
                {   
                    //Actualiza al empleado si ya existe
                    empleado.Nombre = model.Nombre;
                    empleado.Estado = model.Empleado.Estado;

                    _contenedorTrabajo.Empleado.Update(empleado);
                }
            }
            else
            {
                //Borrar al empleado si se cambia de rol
                var empleado = await _contenedorTrabajo.Empleado
                    .GetFirstOrDefaultAsync(e => e.UsuarioId == usuario.Id);

                if (empleado != null)
                    _contenedorTrabajo.Empleado.Remove(empleado);
            }

            await _contenedorTrabajo.SaveAsync();

            TempData["Mensaje"] = $"Se modificó correctamente el usuario con Id: {model.Id}";
            return RedirectToAction("Index");
        }

        // POST: UsuarioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var usuario = await _contenedorTrabajo.Usuario.GetFirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null) return RedirectToAction("Index");

            var supervisor = await _contenedorTrabajo.Supervisor
                .GetFirstOrDefaultAsync(s => s.UsuarioId == id);

            if (supervisor != null)
                _contenedorTrabajo.Supervisor.Remove(supervisor);

            var empleado = await _contenedorTrabajo.Empleado
                .GetFirstOrDefaultAsync(e => e.UsuarioId == id);

            if (empleado != null)
                _contenedorTrabajo.Empleado.Remove(empleado);

            await _userManager.DeleteAsync(usuario);
            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index");
        }

        //Funciones
        private async Task ValidarDuplicidadCampos(string email, string telefono, string? id = null)
        {
            var emailDuplicado = await _contenedorTrabajo.Usuario
                .GetFirstOrDefaultAsync(u =>
                         u.Email == email && (id == null || u.Id != id));

            if (emailDuplicado != null)
            {
                ModelState.AddModelError("Email", "Este correo ya está registrado.");
            }

            var telefonoDuplicado = await _contenedorTrabajo.Usuario
                .GetFirstOrDefaultAsync(u =>
                    u.PhoneNumber == telefono && (id == null || u.Id != id));

            if (telefonoDuplicado != null)
            {
                ModelState.AddModelError("Telefono", "Este teléfono ya está registrado.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEmpresasPorPrograma(int programaId)
        {
            var lista = await _contenedorTrabajo.EmpresaPrograma
                .GetAllAsync(ep => ep.ProgramaId == programaId, includeProperties: "Empresa");

            var resultado = lista.Select(ep => new
            {
                id = ep.Empresa.Id,
                nombre = ep.Empresa.Nombre
            });

            return Json(resultado);
        }
    }
}
