using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaTaskWhatsapp.AccesoDatos.Data.Repository.IRepository;
using SistemaTaskWhatsapp.Data;
using SistemaTaskWhatsapp.Models;
using SistemaTaskWhatsapp.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTaskWhatsapp.AccesoDatos.Data.Repository
{
    public class EmpresaRepository : Repository<Empresa>, IEmpresaRepository
    {
        private readonly ApplicationDbContext _db;

        public EmpresaRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<SelectListItem>> GetEmpresasDropdown()
        {
            var empresas = await _db.Empresa.ToListAsync();

            return empresas.Select(e => new SelectListItem
            {
                Value = e.Id.ToString(),
                Text = e.Nombre
            });
        }

        public async Task<IEnumerable<SelectListItem>> GetEmpresasPorProgramaDropdown(int programaId)
        {
            var lista = await _db.EmpresaPrograma
                .Where(ep => ep.ProgramaId == programaId)
                .Include(ep => ep.Empresa)
                .Select(ep => new SelectListItem
                {
                    Value = ep.Empresa.Id.ToString(),
                    Text = ep.Empresa.Nombre
                })
                .ToListAsync();

            return lista;
        }

    }
}
