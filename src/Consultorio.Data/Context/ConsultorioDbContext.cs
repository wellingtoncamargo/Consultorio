using Consultorio.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Consultorio.Data.Context
{
    public class ConsultorioDbContext : DbContext
    {
        public ConsultorioDbContext(DbContextOptions<ConsultorioDbContext> options) : base(options) { }

        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Medico> Medicos { get; set; }
        public DbSet<LocalTrabalho> LocaisTrabalho { get; set; }
        public DbSet<Agendamento> Agendamentos { get; set; }
        public DbSet<Historico> Historicos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Paciente>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Nome).IsRequired().HasMaxLength(255);
                e.Property(p => p.Email).HasMaxLength(255);
                e.Property(p => p.CPF).HasMaxLength(14);
                e.Property(p => p.Telefone).HasMaxLength(20);
                e.Property(p => p.Celular).HasMaxLength(20);
                e.HasMany(p => p.Agendamentos).WithOne(a => a.Paciente)
                    .HasForeignKey(a => a.PacienteId).OnDelete(DeleteBehavior.Restrict);
                e.HasMany(p => p.Historicos).WithOne(h => h.Paciente)
                    .HasForeignKey(h => h.PacienteId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Medico>(e =>
            {
                e.HasKey(m => m.Id);
                e.Property(m => m.Nome).IsRequired().HasMaxLength(255);
                e.Property(m => m.Email).HasMaxLength(255);
                e.Property(m => m.CPF).HasMaxLength(14);
                e.Property(m => m.CRM).IsRequired().HasMaxLength(20);
                e.Property(m => m.Especialidade).HasMaxLength(255);
                e.HasMany(m => m.LocaisTrabalho).WithOne(l => l.Medico)
                    .HasForeignKey(l => l.MedicoId).OnDelete(DeleteBehavior.Cascade);
                e.HasMany(m => m.Agendamentos).WithOne(a => a.Medico)
                    .HasForeignKey(a => a.MedicoId).OnDelete(DeleteBehavior.Restrict);
                e.HasMany(m => m.Historicos).WithOne(h => h.Medico)
                    .HasForeignKey(h => h.MedicoId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<LocalTrabalho>(e =>
            {
                e.HasKey(l => l.Id);
                e.Property(l => l.Nome).IsRequired().HasMaxLength(255);
                e.Property(l => l.Endereco).HasMaxLength(500);
                e.HasMany(l => l.Agendamentos).WithOne(a => a.LocalTrabalho)
                    .HasForeignKey(a => a.LocalTrabalhoId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Agendamento>(e =>
            {
                e.HasKey(a => a.Id);
                e.Property(a => a.Motivo).HasMaxLength(500);
                e.Property(a => a.Status).HasConversion<int>();
                e.HasIndex(a => new { a.PacienteId, a.DataAgendamento });
                e.HasIndex(a => new { a.MedicoId, a.DataAgendamento });
            });

            modelBuilder.Entity<Historico>(e =>
            {
                e.HasKey(h => h.Id);
                e.Property(h => h.Diagnostico).HasMaxLength(2000);
                e.Property(h => h.Tratamento).HasMaxLength(2000);
                e.Property(h => h.Medicacoes).HasMaxLength(2000);
                e.HasIndex(h => new { h.PacienteId, h.DataConsulta });
            });

            modelBuilder.Entity<Usuario>(e =>
            {
                e.HasKey(u => u.Id);
                e.Property(u => u.Nome).IsRequired().HasMaxLength(255);
                e.Property(u => u.Email).IsRequired().HasMaxLength(255);
                e.Property(u => u.PasswordHash).IsRequired().HasMaxLength(512);
                e.Property(u => u.Role).HasMaxLength(50);
                e.HasIndex(u => u.Email).IsUnique();
            });
        }
    }
}

