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

            //Relación Empresa → Proyecto (CASCADE)
            modelBuilder.Entity<Proyecto>()
                .HasOne(p => p.Empresa)
                .WithMany(e => e.Proyectos)
                .HasForeignKey(p => p.EmpresaId)
                .OnDelete(DeleteBehavior.Cascade);

            //Relación Proyecto → Programa (SIN CASCADE)
            modelBuilder.Entity<Proyecto>()
                .HasOne(p => p.Programa)
                .WithMany()
                .HasForeignKey(p => p.ProgramaId)
                .OnDelete(DeleteBehavior.NoAction);

            //Índice único para IdExterno + Programa
            modelBuilder.Entity<Proyecto>()
                .HasIndex(p => new { p.IdExterno, p.ProgramaId })
                .IsUnique()
                .HasFilter("[IdExterno] IS NOT NULL");

            //Índice único para IdExterno + Empresa
            modelBuilder.Entity<Empresa>()
                .HasIndex(e => new { e.IdExterno, e.ProgramaId })
                .IsUnique()
                .HasFilter("[IdExterno] IS NOT NULL");


            modelBuilder.Entity<Usuario>()
                .HasIndex(u => new { u.IdExterno, u.ProgramaId })
                .IsUnique()
                .HasFilter("[IdExterno] IS NOT NULL");

            modelBuilder.Entity<Tarea>()
                .HasOne(t => t.Proyecto)
                .WithMany(p => p.Tareas)
                .HasForeignKey(t => t.ProyectoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Tarea>()
                .HasIndex(t => new { t.IdExterno, t.ProyectoId })
                .IsUnique()
                .HasFilter("[IdExterno] IS NOT NULL");

            modelBuilder.Entity<Tarea>()
                .HasIndex(t => new { t.Nombre, t.ProyectoId })
                .IsUnique();
        }
    }
}
