using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Data;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Models.ViewModels;

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
            return View();
        }

        // POST: UsuarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SistemaTaskWhatsapp.Models.Usuario nuevoUsuario)
        {
            //Se pone esta linea dos veces para evitar consultar la base de datos si el modelo no es valido de principio
            if (!ModelState.IsValid) return View(nuevoUsuario);

            await ValidarDuplicidadCampos(nuevoUsuario.Email,
                                          nuevoUsuario.Telefono,
                                          nuevoUsuario.Id
                                          );

            if (!ModelState.IsValid) return View(nuevoUsuario);

            await _contenedorTrabajo.Usuario.AddAsync(nuevoUsuario);
            await _contenedorTrabajo.SaveAsync();

            TempData["Mensaje"] = $"Cliente agregado exitosamente Id: {nuevoUsuario.Id} Nombre: {nuevoUsuario.Nombre}";

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
