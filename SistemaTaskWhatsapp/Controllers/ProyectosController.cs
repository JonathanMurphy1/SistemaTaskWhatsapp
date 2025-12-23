using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models.ViewModels;
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

            await _contenedorTrabajo.Proyecto.AddAsync(model.Proyecto);
            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index");
        }
    }
}
