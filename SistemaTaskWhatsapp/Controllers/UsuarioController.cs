using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Data;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Models.ViewModels;
using SistemaTaskWhatsapp.Utilidades;

namespace SistemaTaskWhatsapp.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public UsuarioController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        public async Task<IActionResult> Index()
        {
            var listaUsuarios = await _contenedorTrabajo.Usuario.GetAllAsync();

            return View(listaUsuarios);
        }

        // GET: UsuarioController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UsuarioController/Create
        [HttpGet]
        public IActionResult Create()
        {
            var model = new UsuarioCreateVM
            {
                Supervisor = new Supervisor(),
                Empleado = new Empleado()
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

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario = new Usuario
            {
                Nombre = model.Nombre,
                Email = model.Email,
                Password = model.Password,
                Telefono = model.Telefono,
                Rol = model.Rol
            };

            await _contenedorTrabajo.Usuario.AddAsync(usuario);
            await _contenedorTrabajo.SaveAsync();

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
        public async Task<IActionResult> Edit(int id)
        {
            var usuario = await _contenedorTrabajo.Usuario.GetByIdAsync(id);
            if (usuario == null)
                return RedirectToAction("Index");

            var model = new EditarUsuarioVM
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Password = usuario.Password,
                Telefono = usuario.Telefono,
                Rol = usuario.Rol,
                Supervisor = new Supervisor(),
                Empleado = new Empleado()
            };

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
                return View(model);

            await ValidarDuplicidadCampos(model.Email, model.Telefono, model.Id);

            if (!ModelState.IsValid)
                return View(model);

            var usuario = await _contenedorTrabajo.Usuario.GetByIdAsync(model.Id);
            if (usuario == null)
            {
                TempData["Mensaje"] = "Usuario no encontrado";
                TempData["Error"] = "Error";
                return RedirectToAction("Index");
            }

            //Usuario
            usuario.Nombre = model.Nombre;
            usuario.Email = model.Email;
            usuario.Telefono = model.Telefono;
            usuario.Password = model.Password;
            usuario.Rol = model.Rol;

            _contenedorTrabajo.Usuario.Update(usuario);

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
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await _contenedorTrabajo.Usuario.GetByIdAsync(id);
            if (usuario == null) return RedirectToAction("Index");

            var supervisor = await _contenedorTrabajo.Supervisor
                .GetFirstOrDefaultAsync(s => s.UsuarioId == id);

            if (supervisor != null)
                _contenedorTrabajo.Supervisor.Remove(supervisor);

            var empleado = await _contenedorTrabajo.Empleado
                .GetFirstOrDefaultAsync(e => e.UsuarioId == id);

            if (empleado != null)
                _contenedorTrabajo.Empleado.Remove(empleado);

            _contenedorTrabajo.Usuario.Remove(usuario);
            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index");
        }

        //Funciones
        private async Task ValidarDuplicidadCampos(string email, string telefono, int? id = null)
        {
            var duplicado = await _contenedorTrabajo.Usuario.GetFirstOrDefaultAsync(u =>
                (u.Email == email || u.Telefono == telefono) &&
                u.Id != id.Value);

            if (duplicado != null)
            {
                if (duplicado.Email == email)
                    ModelState.AddModelError("Email", "Este correo ya está registrado.");

                if (duplicado.Telefono == telefono)
                    ModelState.AddModelError("Telefono", "Este teléfono ya está registrado.");
            }
        }
    }
}
