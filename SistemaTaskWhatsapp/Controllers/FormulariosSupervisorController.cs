using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Mvc;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Models.ViewModels;
using SistemaTaskWhatsapp.Utilidades;
using Twilio.Jwt.AccessToken;
using Twilio.TwiML.Messaging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace SistemaTaskWhatsapp.Controllers
{
    public class FormulariosSupervisorController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public FormulariosSupervisorController(IContenedorTrabajo contenedorTrabajo)
        {
                _contenedorTrabajo = contenedorTrabajo;
        }

        [HttpGet]
        public IActionResult Resultado(string mensaje, bool resultado)
        {
            var model = new ResultadoFormSupervisorVM
            {
                Mensaje = mensaje,
                Resultado = resultado
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> FormularioEditarTarea(int id, string token)
        {
            var validacion = await ValidarToken(token);
            if (validacion != null)
                return validacion;

            var tarea = await _contenedorTrabajo.Tarea.GetFirstOrDefaultAsync(t => t.Id == id && t.Estado == EstadosTarea.Pendiente);

            if(tarea == null)
                return RedirectToAction("Resultado", new {mensaje = "Error al buscar la tarea", resultado = false});

            var vm = new EditarTareaFormSupervisorVM
            {
                Tarea = tarea,
                Token = token
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FormularioEditarTarea(EditarTareaFormSupervisorVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var validacion = await ValidarToken(model.Token);
            if (validacion != null)
                return validacion;

            string mensaje = "";
            bool resultado = false;

            var tareaBd = await _contenedorTrabajo.Tarea.GetFirstOrDefaultAsync(t => t.Id == model.Tarea.Id && t.Estado == EstadosTarea.Pendiente);

            if (tareaBd == null)
            {
                mensaje = "No se encontro la tarea en la base de datos";
                return RedirectToAction("Resultado", new { mensaje = mensaje, resultado = resultado });
            }

            if (model.Tarea.FechaEntrega < DateTime.Now)
            {
                ModelState.AddModelError("Fechaentrega", "La fecha de entrega debe ser mayor a la actual");
                return View(model);
            }

            tareaBd.Nombre = model.Tarea.Nombre;
            tareaBd.Descripcion = model.Tarea.Descripcion;
            tareaBd.FechaEntrega = model.Tarea.FechaEntrega;

            _contenedorTrabajo.Tarea.Update(tareaBd);
            await _contenedorTrabajo.SaveAsync();

            mensaje = "Tarea editada correctamente";
            resultado = true;

            return RedirectToAction("Resultado", new {mensaje = mensaje, resultado = resultado });
        }

        [HttpGet]
        public async Task<IActionResult> FormularioCrearProyecto(string token)
        {
            var validacion = await ValidarToken(token);
            if (validacion != null)
                return validacion;

            var model = new NuevoProyectoFormSupervisorVM
            {
                ListaEmpresas = await _contenedorTrabajo.Empresa.GetEmpresasDropdown(),
                Token = token
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FormularioCrearProyecto(NuevoProyectoFormSupervisorVM model)
        {
            var validacion = await ValidarToken(model.Token);
            if (validacion != null)
                return validacion;

            if(!ModelState.IsValid)
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

            await _contenedorTrabajo.Proyecto.AddAsync(model.Proyecto);
            await _contenedorTrabajo.SaveAsync();

            return RedirectToAction("Resultado", new {mensaje = "Proyecto creado correctamente", resultado = true});
        }

        [HttpGet]
        public async Task<IActionResult> AsignarColaboradores(int id, string token)
        {
            var validacion = await ValidarToken(token);
            if (validacion != null)
                return validacion;

            var tarea = await _contenedorTrabajo.Tarea.GetFirstOrDefaultAsync(t => t.Id == id, includeProperties: "TareaEmpleados.Empleado.Usuario");

            if(tarea == null)
                return RedirectToAction("Resultado", new { mensaje = "No se encontro la tarea", resultado = false });

            var model = new ColaboradoresFormSupervisorVM
            {
                ListaEmpleados = await _contenedorTrabajo.Empleado.ObtenerListaEmpleados(),
                Token = token,
                Tarea = tarea
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarColaboradores(int? tareaId, int? colaboradorId, string? token)
        {
            var validacion = await ValidarToken(token);
            if (validacion != null)
                return validacion;

            if (tareaId == null || colaboradorId == null)
            {
                return RedirectToAction("Resultado", new { mensaje = "Error al buscar el Id de la tarea", resultado = false });
            }

            if (await _contenedorTrabajo.TareaEmpleado.
                GetFirstOrDefaultAsync(te => te.EmpleadoId == colaboradorId && te.TareaId == tareaId) != null)
            {
                return RedirectToAction("Resultado", new { mensaje = "Este empleado ya esta colaborando en esta tarea", resultado = false });
            }

            var nuevoColaborador = new TareaEmpleado
            {
                TareaId = (int)tareaId,
                EmpleadoId = (int)colaboradorId
            };

            await _contenedorTrabajo.TareaEmpleado.AddAsync(nuevoColaborador);
            await _contenedorTrabajo.SaveAsync();
            return RedirectToAction("Resultado", new { mensaje = "Colaborador añadido correctamente", resultado = true });
        }

        //Funciones
        private async Task<IActionResult?> ValidarToken(string? token)
        {
            if (token == null)
                return RedirectToAction("Resultado", new { mensaje = "No se envio el token", resultado = false });

            var sesion = await _contenedorTrabajo.ChatSession
                .GetFirstOrDefaultAsync(c => c.TokenFormularios == token);

            if (sesion == null)
                return RedirectToAction("Resultado", new { mensaje = "Token no valido", resultado = false });

            if (sesion.FechaCreacionToken == null || sesion.FechaCreacionToken.Value.AddMinutes(15) < DateTime.Now)
                return RedirectToAction("Resultado", new { mensaje = "Token caducado", resultado = false });

            return null;
        }
    }
}
