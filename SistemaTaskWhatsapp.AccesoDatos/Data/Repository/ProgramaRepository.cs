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
    public class ProgramaRepository : Repository<Programa>, IProgramaRepository
    {
        private readonly ApplicationDbContext _db;

        public ProgramaRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<SelectListItem>> GetProgramaDropdown(int? id = null)
        {
            return await _db.Programa.Select(e => new SelectListItem
            {
                Text = e.Nombre,
                Value = e.Id.ToString()
            }).ToListAsync();
        }
    }
}
