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

        // GET: UsuarioController
        public ActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetAll()
        {
            var usuarios = await _contenedorTrabajo.Usuario.GetAllAsync();

            var listaUsuarios = usuarios.Select(u => new
            {
                id = u.Id,
                nombre = u.Nombre,
                email = u.Email,
                telefono = u.Telefono,
                rol = u.Rol.ToString()
            }).ToList();

            return Json(new { data = listaUsuarios });
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
                Supervisor = new Supervisor()
            };
            return View(model);
        }

        // POST: UsuarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioCreateVM model)
        {
            if (model.Supervisor == null)
            {
                model.Supervisor = new Models.Supervisor();
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
                var nuevoSupervisor = new Models.Supervisor
                {
                    Nombre = usuario.Nombre,
                    UsuarioId = usuario.Id,
                    Estado = model.Supervisor.Estado
                };
                

                await _contenedorTrabajo.Supervisor.AddAsync(nuevoSupervisor);
                await _contenedorTrabajo.SaveAsync();
            }

            TempData["Mensaje"] = $"Usuario con el nombre: {usuario.Nombre}";
            return RedirectToAction("Index");

        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var usuario = await _contenedorTrabajo.Usuario.GetByIdAsync(id);

            if (usuario == null)
            {
                TempData["Mensaje"] = "No se encontró el usuario";
                TempData["Error"] = "Error";
                return RedirectToAction("Index");
            }

            var model = new EditarUsuarioVM
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Password = usuario.Password,
                Telefono = usuario.Telefono,
                Rol = usuario.Rol
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditarUsuarioVM model)
        {
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

            usuario.Nombre = model.Nombre;
            usuario.Email = model.Email;
            usuario.Telefono = model.Telefono;
            usuario.Password = model.Password;
            usuario.Rol = model.Rol;

            _contenedorTrabajo.Usuario.Update(usuario);
            await _contenedorTrabajo.SaveAsync();

            TempData["Mensaje"] = $"Se modificó correctamente el usuario con Id: {model.Id}";
            return RedirectToAction("Index");
        }

        // POST: UsuarioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            bool resultado = await _contenedorTrabajo.Usuario.RemoveByIdAsync(id);

            if (!resultado)
            {
                TempData["Mensaje"] = $"Hubo un error al tratar de borrar el usuario Id: {id}";
                TempData["error"] = "Error";
                return RedirectToAction("Index");
            }

            await _contenedorTrabajo.SaveAsync();

            TempData["Mensaje"] = $"Usuario borrado correctamente Id: {id}";

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
