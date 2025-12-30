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
    public class ReporteRepository : Repository<Reporte>, IReporteRepository
    {
        private readonly ApplicationDbContext _db;

        public ReporteRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
