using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Utilidades;

namespace SistemaTaskWhatsapp.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class EmpleadosController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public EmpleadosController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var listaEmpleados = await _contenedorTrabajo.Empleado.GetAllAsync();

            return View(listaEmpleados);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Empleado model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existeNombre = await _contenedorTrabajo.Empleado.GetFirstOrDefaultAsync(e => e.Id == model.Id);

            if (existeNombre != null)
            {
                ModelState.AddModelError("", "Ya existe un empleado con ese id");
                return View(model);
            }

            await _contenedorTrabajo.Empleado.AddAsync(model);
            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _contenedorTrabajo.Empleado.GetByIdAsync(id);

            if (model == null) return RedirectToAction("Index");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Empleado model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _contenedorTrabajo.Empleado.Update(model);
            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var EmpleadoEliminar = await _contenedorTrabajo.Empleado.GetByIdAsync(id);

            if (EmpleadoEliminar == null) return RedirectToAction("Index");

            _contenedorTrabajo.Empleado.Remove(EmpleadoEliminar);
            await _contenedorTrabajo.SaveAsync();
            return RedirectToAction("Index");

        }

        [HttpPost]
        public async Task<IActionResult> CambiarEstado(int id, EstadosEmpleado estado)
        {
            var empleado = await _contenedorTrabajo.Empleado.GetFirstOrDefaultAsync(e => e.Id == id);

            if (empleado == null)
                return NotFound();

            empleado.Estado = estado;

            _contenedorTrabajo.Empleado.Update(empleado);
            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
