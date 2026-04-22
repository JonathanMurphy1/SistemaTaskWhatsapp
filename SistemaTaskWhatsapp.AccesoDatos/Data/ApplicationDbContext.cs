using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaTaskWhatsapp.Models;

namespace SistemaTaskWhatsapp.Data
{
    public class ApplicationDbContext : IdentityDbContext<Usuario>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //Poner aqui todos los modelos que se vayan creando
        public DbSet<Supervisor> Supervisor { get; set; }
        public DbSet<Empleado> Empleado { get; set; }
        public DbSet<Empresa> Empresa { get; set; }
        public DbSet<Proyecto> Proyecto { get; set; }
        public DbSet<Tarea> Tarea { get; set; }
        public DbSet<Reporte> Reporte { get; set; }
        public DbSet<Evidencia> Evidencia { get; set; }
        public DbSet<ChatSession> ChatSession { get; set; }

        public DbSet<TareaEmpleado> TareaEmpleado { get;set; }
        public DbSet<Retroalimentacion> Retroalimentacion { get; set; }

        public DbSet<DiaFestivo> DiaFestivo { get; set; }

        public DbSet<Mensaje> Mensaje { get; set; }

        public DbSet<MensajeDiaFestivo> MensajeDiaFestivo { get; set; }
        public DbSet<Programa> Programa { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Índice único compuesto
            modelBuilder.Entity<Empresa>()
                .HasIndex(e => new { e.Nombre, e.ProgramaId })
                .IsUnique();
        }
    }
}
