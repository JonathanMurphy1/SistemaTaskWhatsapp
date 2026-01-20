using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Models.ViewModels;
using SistemaTaskWhatsapp.Utilidades;

namespace SistemaTaskWhatsapp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public HomeController(ILogger<HomeController> logger, IContenedorTrabajo contenedorTrabajo)
        {
            _logger = logger;
            _contenedorTrabajo = contenedorTrabajo;
        }

        public async Task<IActionResult> Index()
        {
            if (!(User.Identity.IsAuthenticated && (User.IsInRole("Administrador") || User.IsInRole("Supervisor"))))
                return View();

            var lista = await _contenedorTrabajo.Proyecto.GetAllAsync(includeProperties: "Empresa");

            var model = lista.Select(x => new ProyectoHomeVM
            {
                Proyecto = x,
                TareasPendientes = _contenedorTrabajo.Tarea.
                    GetAllQueryable(t => t.ProyectoId == x.Id && t.Estado == EstadosTarea.Pendiente).
                    Count(),
                ReportesPendientesRevisar = _contenedorTrabajo.Reporte.
                    GetAllQueryable(r => r.Tarea.ProyectoId == x.Id && r.Estado == EstadosReporte.PendienteRevisar).
                    Count()
            });

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
