using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Data;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.AccesoDatos.Data.Repository
{
    public class TareaRepository : Repository<Tarea>, ITareaRepository
    {
        private readonly ApplicationDbContext _db;

        public TareaRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<SelectListItem>> ObtenerTareasVigentes()
        {
            return await _db.Tarea.Where(t => t.Estado != EstadosTarea.Finalizada).
                Select(t => new SelectListItem
                {
                    Text = t.Nombre,
                    Value = t.Id.ToString()
                }).ToListAsync();
        }
    }
}
