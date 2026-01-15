using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models.ViewModels;
using SistemaTaskWhatsapp.Utilidades;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Controllers
{
    public class ProyectosController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public ProyectosController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var listaProyectos = await _contenedorTrabajo.Proyecto.GetAllAsync(includeProperties:"Empresa");
            return View(listaProyectos);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new ProyectoVM
            {
                ListaEmpresas = await _contenedorTrabajo.Empresa.GetEmpresasDropdown()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProyectoVM model)
        {
            if (!ModelState.IsValid)
            {
                model.ListaEmpresas = await _contenedorTrabajo.Empresa.GetEmpresasDropdown();
                return View(model);
            }

            var mismoNombre = await _contenedorTrabajo.Proyecto.GetFirstOrDefaultAsync(p => p.Nombre == model.Proyecto.Nombre);
            if(mismoNombre != null)
            {
                ModelState.AddModelError("", "Ya existe un proyecto con ese nombre");
                model.ListaEmpresas = await _contenedorTrabajo.Empresa.GetEmpresasDropdown();
                return View(model);
            }

            model.Proyecto.FechaRegistro = DateTime.Now;

            await _contenedorTrabajo.Proyecto.AddAsync(model.Proyecto);
            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var proyecto = await _contenedorTrabajo.Proyecto.GetByIdAsync(id);

            if(proyecto == null)
            {
                return RedirectToAction("Index");
            }

            var model = new ProyectoVM
            {
                Proyecto = proyecto,
                ListaEmpresas = await _contenedorTrabajo.Empresa.GetEmpresasDropdown()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProyectoVM model)
        {
            if (!ModelState.IsValid)
            {
                model.ListaEmpresas = await _contenedorTrabajo.Empresa.GetEmpresasDropdown();
                return View(model);
            }

            var mismoNombre = await _contenedorTrabajo.Proyecto.GetFirstOrDefaultAsync(p => p.Id != model.Proyecto.Id && p.Nombre == model.Proyecto.Nombre);
            if (mismoNombre != null)
            {
                ModelState.AddModelError("", "Ya existe un proyecto con ese nombre");
                model.ListaEmpresas = await _contenedorTrabajo.Empresa.GetEmpresasDropdown();
                return View(model);
            }

            _contenedorTrabajo.Proyecto.Update(model.Proyecto);
            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Finalizar(int id)
        {
            var proyecto = await _contenedorTrabajo.Proyecto.GetByIdAsync(id);

            if (proyecto == null || proyecto.Estado == EstadosProyecto.Terminado) return RedirectToAction("Index");

            proyecto.FechaFin = DateTime.Now;
            proyecto.Estado = EstadosProyecto.Terminado;

            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var proyecto = await _contenedorTrabajo.Proyecto.GetByIdAsync(id);

            if (proyecto == null) return RedirectToAction("Index");

            _contenedorTrabajo.Proyecto.Remove(proyecto);

            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index");
        }


    }
}
