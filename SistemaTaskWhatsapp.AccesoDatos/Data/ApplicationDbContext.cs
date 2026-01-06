using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaTaskWhatsapp.Models;

namespace SistemaTaskWhatsapp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //Poner aqui todos los modelos que se vayan creando
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Supervisor> Supervisor { get; set; }
        public DbSet<Empleado> Empleado { get; set; }
        public DbSet<Empresa> Empresa { get; set; }
        public DbSet<Proyecto> Proyecto { get; set; }
        public DbSet<Tarea> Tarea { get; set; }
        public DbSet<Reporte> Reporte { get; set; }
        public DbSet<TareaEmpleado> TareaEmpleado { get;set; }
    }
}
