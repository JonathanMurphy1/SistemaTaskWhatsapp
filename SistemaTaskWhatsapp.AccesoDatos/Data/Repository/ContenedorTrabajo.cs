using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Data;
using SistemaTaskWhatsapp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.AccesoDatos.Data.Repository
{
    public class ContenedorTrabajo : IContenedorTrabajo
    {
        private readonly ApplicationDbContext _db;
        public ContenedorTrabajo(ApplicationDbContext db)
        {
            _db = db;  
            Usuario = new UsuarioRepository(_db);
            Supervisor = new SupervisorRepository(_db);
            Empleado = new EmpleadoRepository(_db);
            Empresa = new EmpresaRepository(_db);
            Proyecto = new ProyectoRepository(_db);
            Tarea = new TareaRepository(_db);
            Reporte = new ReporteRepository(_db);
            Evidencia = new EvidenciaRepository(_db);
            TareaEmpleado = new TareaEmpleadoRepository(_db);
        }

        public IUsuarioRepository Usuario {  get; private set; }
        public IEmpresaRepository Empresa { get; private set; }
        public IProyectoRepository Proyecto { get; private set; }
        public ITareaRepository Tarea { get; private set; }
        public IReporteRepository Reporte { get; private set; }
        public ISupervisorRepository Supervisor { get; private set; }
        public IEmpleadoRepository Empleado { get; private set; }
        public IEvidenciaRepository Evidencia { get; private set; }
        public ITareaEmpleadoRepository TareaEmpleado { get; private set; }

        public void Dispose()
        {
            _db.Dispose();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
