using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Controllers
{
    public class EmpresasController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public EmpresasController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo; 
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var listaEmpresas = await _contenedorTrabajo.Empresa.GetAllAsync();

            return View(listaEmpresas);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Empresa model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var existeNombre = await _contenedorTrabajo.Empresa.GetFirstOrDefaultAsync(e => e.Nombre == model.Nombre);

            if(existeNombre != null)
            {
                ModelState.AddModelError("", "Ya existe una empresa con ese nombre");
                return View(model);
            }

            model.FechaRegistro = DateTime.Now;

            await _contenedorTrabajo.Empresa.AddAsync(model);
            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _contenedorTrabajo.Empresa.GetByIdAsync(id);

            if (model == null) return RedirectToAction("Index");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Empresa model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existeNombre = await _contenedorTrabajo.Empresa.GetFirstOrDefaultAsync(e => e.Nombre == model.Nombre && e.Id != model.Id);

            if (existeNombre != null)
            {
                ModelState.AddModelError("", "Ya existe una empresa con ese nombre");
                return View(model);
            }

            _contenedorTrabajo.Empresa.Update(model);
            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var empresaEliminar = await _contenedorTrabajo.Empresa.GetFirstOrDefaultAsync(e => e.Id == id, includeProperties:"Proyectos");

            if(empresaEliminar == null) return RedirectToAction("Index");

            foreach(var proyecto in empresaEliminar.Proyectos)
            {
                proyecto.EmpresaId = null;
            }

            _contenedorTrabajo.Empresa.Remove(empresaEliminar);
            await _contenedorTrabajo.SaveAsync();
            return RedirectToAction("Index");
        }
    }
}
