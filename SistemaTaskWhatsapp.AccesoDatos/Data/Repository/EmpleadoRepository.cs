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
    public class EmpleadoRepository : Repository<Empleado>, IEmpleadoRepository
    {
        private readonly ApplicationDbContext _db;

        public EmpleadoRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<SelectListItem>> ObtenerListaEmpleados()
        {
            return await _db.Empleado.Where(e => e.Estado != Utilidades.EstadosEmpleado.Inactivo).
                Select(es => new SelectListItem
                {
                    Text = $"{es.Nombre} (Email: {es.Usuario.Email})",
                    Value = es.Id.ToString()
                }).ToListAsync();
        }
    }
}
