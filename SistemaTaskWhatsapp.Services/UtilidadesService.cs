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

            //Buscar en BD al iniciar
            var festivosBD = await _contenedorTrabajo.DiaFestivo
                .GetAllAsync(f => f.Date.Year == year);

            if (festivosBD != null && festivosBD.Any())
            {
                return festivosBD.ToList();
            }

            //Si no hay días guardados en BD llamar a la API
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

                foreach (var festivo in festivosApi)
                {
                    await _contenedorTrabajo.DiaFestivo.AddAsync(festivo);
                }

                await _contenedorTrabajo.SaveAsync();

                return festivosApi;
            }
        }

        //Revisa si el día actual es festivo o no
        public async Task<bool> EsDiaFestivo()
        {
            var festivos = await ObtenerFestivos();
            var hoy = DateTime.Now.Date;

            return festivos.Any(f => f.Date.Date == hoy);
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