using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository
{
    public interface IContenedorTrabajo : IDisposable
    {
        //Aqui se van agregando los diferentes repositorios
        IUsuarioRepository Usuario { get; }
        IEmpresaRepository Empresa { get; }
        IProyectoRepository Proyecto { get; }
        ITareaRepository Tarea { get; }
        IReporteRepository Reporte { get; }
        ISupervisorRepository Supervisor { get; }
        IEmpleadoRepository Empleado { get; }
        IEvidenciaRepository Evidencia { get; }
        ITareaEmpleadoRepository TareaEmpleado { get; }
        IRetroalimentacionRepository Retroalimentacion { get; }
        IChatSessionRepository ChatSession { get; }
        IDiaFestivoRepository DiaFestivo { get; }
        IMensajeRepository Mensaje { get; }
        IMensajeDiaFestivoRepository MensajeDiaFestivo { get; }

        Task SaveAsync();
    }
}
