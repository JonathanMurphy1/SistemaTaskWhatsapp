using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<SelectListItem>> ListaReportes()
        {
            return await _db.Reporte.Where(x => x.Estado == Utilidades.EstadosReporte.PendienteRevisar).Select(p => new SelectListItem
            {
                Text = p.Nombre,
                Value = p.Id.ToString(),
            }).ToListAsync();
        }
    }
}
