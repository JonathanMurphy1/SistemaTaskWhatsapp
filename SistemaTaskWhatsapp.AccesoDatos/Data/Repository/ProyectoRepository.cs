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
    public class ProyectoRepository : Repository<Proyecto>, IProyectoRepository
    {
        private readonly ApplicationDbContext _db;

        public ProyectoRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<SelectListItem>> ObtenerProyectosVigentes()
        {
            return await _db.Proyecto.Where(x => x.Estado != Utilidades.EstadosProyecto.Terminado).Select(p => new SelectListItem
            {
                Text = p.Nombre,
                Value = p.Id.ToString(),
            }).ToListAsync();
        }
    }
}
