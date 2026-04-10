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
    public class MensajeDiaFestivoRepository : Repository<MensajeDiaFestivo>, IMensajeDiaFestivoRepository
    {
        private readonly ApplicationDbContext _db;

        public MensajeDiaFestivoRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<SelectListItem>> ObtenerDiasFestivos()
        {
            var dias = await _db.DiaFestivo.ToListAsync();

            return dias.Select(d => new SelectListItem
            {
                Text = $"{d.LocalName} ({d.Date:dd/MM/yyyy})",
                Value = d.Id.ToString()
            });
        }
    }
}
