using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Models.ViewModels;
using SistemaTaskWhatsapp.Utilidades;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Controllers
{
    public class EvidenciasController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EvidenciasController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var listaEvidencias = await _contenedorTrabajo.Evidencia.GetAllAsync(includeProperties: "Reporte");
            return View(listaEvidencias);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new EvidenciaVM
            {
                Evidencia = new Evidencia(),
                ListaReportes = await _contenedorTrabajo.Reporte.ListaReportes()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EvidenciaVM model, IFormFile archivo)
        {
            ModelState.Remove("Evidencia.Url");
            ModelState.Remove("archivo");

            if (!ModelState.IsValid)
            {
                model.ListaReportes = await _contenedorTrabajo.Reporte.ListaReportes();
                return View(model);
            }

            if (archivo == null || archivo.Length == 0)
            {
                ModelState.AddModelError("", "Debe seleccionar un archivo");
                model.ListaReportes = await _contenedorTrabajo.Reporte.ListaReportes();
                return View(model);
            }

            //Crear la ruta fisica de la carpeta
            string carpetaDestino = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "evidencias"
            );

            if (!Directory.Exists(carpetaDestino))
                Directory.CreateDirectory(carpetaDestino);

            string extension = Path.GetExtension(archivo.FileName).ToLower();
            string nombreArchivo = Guid.NewGuid().ToString() + extension;
            string rutaCompleta = Path.Combine(carpetaDestino, nombreArchivo);

            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".doc", ".docx" };
            if (!extensionesPermitidas.Contains(extension))
            {
                ModelState.AddModelError("", "Tipo de archivo no permitido");
                model.ListaReportes = await _contenedorTrabajo.Reporte.ListaReportes();
                return View(model);
            }

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            model.Evidencia.Url = "/uploads/evidencias/" + nombreArchivo;

            await _contenedorTrabajo.Evidencia.AddAsync(model.Evidencia);
            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var evidencia = await _contenedorTrabajo.Evidencia.GetByIdAsync(id);

            if (evidencia == null)
            {
                return RedirectToAction("Index");
            }

            var model = new EvidenciaVM
            {
                Evidencia = evidencia,
                ListaReportes = await _contenedorTrabajo.Reporte.ListaReportes()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EvidenciaVM model)
        {
            if (!ModelState.IsValid)
            {
                model.ListaReportes = await _contenedorTrabajo.Reporte.ListaReportes();
                return View(model);
            }

            _contenedorTrabajo.Evidencia.Update(model.Evidencia);
            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index");

        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var evidencia = await _contenedorTrabajo.Evidencia.GetByIdAsync(id);

            if (evidencia == null)
                return RedirectToAction("Index");

            //Ruta fisica del archivo
            if (!string.IsNullOrEmpty(evidencia.Url))
            {
                string rutaFisica = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    evidencia.Url.TrimStart('/')
                );

                if (System.IO.File.Exists(rutaFisica))
                {
                    System.IO.File.Delete(rutaFisica);
                }
            }

            _contenedorTrabajo.Evidencia.Remove(evidencia);
            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Index");
        }

    }
}
