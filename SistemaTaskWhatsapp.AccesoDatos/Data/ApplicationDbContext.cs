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

        public DbSet<EmpresaPrograma> EmpresaPrograma { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EmpresaPrograma>()
                .HasOne(ep => ep.Empresa)
                .WithMany(e => e.EmpresaProgramas)
                .HasForeignKey(ep => ep.EmpresaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmpresaPrograma>()
                .HasOne(ep => ep.Programa)
                .WithMany(p => p.EmpresaProgramas)
                .HasForeignKey(ep => ep.ProgramaId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<EmpresaPrograma>()
                .HasIndex(ep => new { ep.EmpresaId, ep.ProgramaId })
                .IsUnique();

            // Empresa
            modelBuilder.Entity<Empresa>()
                .HasOne(e => e.ProgramaOrigen)
                .WithMany()
                .HasForeignKey(e => e.ProgramaOrigenId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Empresa>()
                .HasIndex(e => e.Nombre)
                .IsUnique(); // opcional (evita duplicados por nombre)

            // Proyecto -> empresa (cliente)
            modelBuilder.Entity<Proyecto>()
                .HasOne(p => p.Empresa)
                .WithMany(e => e.Proyectos)
                .HasForeignKey(p => p.EmpresaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Proyecto -> Programa (origen)
            modelBuilder.Entity<Proyecto>()
                .HasOne(p => p.Programa)
                .WithMany()
                .HasForeignKey(p => p.ProgramaId)
                .OnDelete(DeleteBehavior.NoAction);

            // Indice proyecto (integración)
            modelBuilder.Entity<Proyecto>()
                .HasIndex(p => new { p.IdExterno, p.ProgramaId })
                .IsUnique()
                .HasFilter("[IdExterno] IS NOT NULL");

            //Usuario -> Empresa
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Empresa)
                .WithMany(e => e.Usuarios)
                .HasForeignKey(u => u.EmpresaId)
                .OnDelete(DeleteBehavior.SetNull);

            //Índice opcional para usuarios externos
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.IdExterno)
                .HasFilter("[IdExterno] IS NOT NULL");

            // Tarea -> proyecto
            modelBuilder.Entity<Tarea>()
                .HasOne(t => t.Proyecto)
                .WithMany(p => p.Tareas)
                .HasForeignKey(t => t.ProyectoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indices tarea
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
