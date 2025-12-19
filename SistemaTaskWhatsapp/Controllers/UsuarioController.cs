using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Data;
using SistemaTaskWhatsapp.Models;

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


            if (!ModelState.IsValid) return View(nuevoUsuario);

            await _contenedorTrabajo.Usuario.AddAsync(nuevoUsuario);
            await _contenedorTrabajo.SaveAsync();

            TempData["Mensaje"] = $"Cliente agregado exitosamente Id: {nuevoUsuario.Id} Nombre: {nuevoUsuario.Nombre}";

            return RedirectToAction("Index");
        }

        // GET: UsuarioController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UsuarioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UsuarioController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UsuarioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
