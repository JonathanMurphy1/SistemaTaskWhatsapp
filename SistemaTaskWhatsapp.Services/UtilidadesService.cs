using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Utilidades;
using SistemaTaskWhatsapp.Utilidades;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.Services
{
    public class UtilidadesService
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public UtilidadesService(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        //Funcion para obtener días festivos mediante una API y guardarlos en la BD
        public async Task<List<DiaFestivo>> ObtenerFestivos()
        {
            int year = DateTime.Now.Year;

            //Validación inicial para evitar hacer todo el proceso en cada llamada
            var festivosActuales = await _contenedorTrabajo.DiaFestivo
               .GetAllAsync(f => f.Date.Year == year);

            if (festivosActuales.Any())
            {
                return festivosActuales.ToList();
            }

            //Obtener días de años anteriores
            var festivosViejos = await _contenedorTrabajo.DiaFestivo
                .GetAllAsync(f => f.Date.Year < year);

            var idsViejos = festivosViejos.Select(f => f.Id).ToList();

            //Eliminar relaciones mensaje-dia usando IDs
            var relaciones = await _contenedorTrabajo.MensajeDiaFestivo
                .GetAllAsync(r => idsViejos.Contains(r.DiaFestivoId));

            foreach (var r in relaciones)
            {
                _contenedorTrabajo.MensajeDiaFestivo.Remove(r);
            }

            //Eliminar días de años anteriores
            foreach (var v in festivosViejos)
            {
                _contenedorTrabajo.DiaFestivo.Remove(v);
            }

            await _contenedorTrabajo.SaveAsync();

            //Llamar a la API
            using (var http = new HttpClient())
            {
                var url = $"https://date.nager.at/api/v3/PublicHolidays/{year}/MX";
                var response = await http.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Error al consultar API");
                    return new List<DiaFestivo>();
                }

                var json = await response.Content.ReadAsStringAsync();

                var festivosApi = JsonSerializer.Deserialize<List<DiaFestivo>>(json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (festivosApi == null)
                    return new List<DiaFestivo>();

                foreach (var festivo in festivosApi)
                {
                    //Evitar días duplicados
                    var existe = await _contenedorTrabajo.DiaFestivo
                        .GetFirstOrDefaultAsync(x => x.Date == festivo.Date);

                    if (existe == null)
                    {
                        await _contenedorTrabajo.DiaFestivo.AddAsync(new DiaFestivo
                        {
                            Date = festivo.Date,
                            LocalName = festivo.LocalName
                        });
                    }
                }

                await _contenedorTrabajo.SaveAsync();

                return festivosApi;
            }
        }

        //Revisa si el día actual es festivo o no del mensaje correspondiente
        public async Task<bool> EsDiaFestivo(int mensajeId)
        {
            var hoy = DateTime.Now.Date;

            var festivos = await _contenedorTrabajo.MensajeDiaFestivo
                .GetAllAsync(x => x.MensajeId == mensajeId, includeProperties: "DiaFestivo");

            return festivos.Any(f => f.DiaFestivo.Date.Date == hoy);
        }

        //Cargar los datos cuando se usen variables en los mensajes
        public string ProcesarPlantilla(string plantilla, Dictionary<string, string> valores)
        {
            if (string.IsNullOrEmpty(plantilla))
                return "";

            foreach (var item in valores)
            {
                plantilla = plantilla.Replace($"{{{item.Key}}}", item.Value ?? "");
            }

            return plantilla;
        }

        //Obtener tareas pendientes
        public async Task<int> ObtenerTareasPendientes(int empleadoId)
        {
            var tareas = await _contenedorTrabajo.TareaEmpleado
                .GetAllAsync(includeProperties: "Tarea");

            return tareas.Count(t =>
                t.EmpleadoId == empleadoId &&
                t.Tarea.Estado == EstadosTarea.Pendiente
            );
        }

        //Obtener reportes pendientes
        public async Task<int> ObtenerReportesPendientes()
        {
            var reportes = await _contenedorTrabajo.Reporte
                .GetAllAsync(r => r.Estado == EstadosReporte.PendienteRevisar);

            return reportes.Count();
        }
    }
}