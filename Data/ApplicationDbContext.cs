using Microsoft.EntityFrameworkCore;
using Api.Models;

namespace EFEnergiaAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Setor> Setores { get; set; }
        public DbSet<Equipamento> Equipamentos { get; set; }
        public DbSet<Leitura> Leituras { get; set; }
        public DbSet<Alerta> Alertas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Setor>(entity =>
            {
                entity.ToTable("Setores");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(120);

                entity.HasMany(e => e.Equipamentos)
                      .WithOne(eq => eq.Setor)
                      .HasForeignKey(eq => eq.SetorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Equipamento>(entity =>
            {
                entity.ToTable("Equipamentos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(120);

                entity.HasIndex(e => new { e.Nome, e.SetorId }).IsUnique();

                entity.HasMany(e => e.Leituras)
                      .WithOne(l => l.Equipamento)
                      .HasForeignKey(l => l.EquipamentoId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Alertas)
                      .WithOne(a => a.Equipamento)
                      .HasForeignKey(a => a.EquipamentoId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Leitura>(entity =>
            {
                entity.ToTable("Leituras");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Temperatura)
                      .IsRequired()
                      .HasPrecision(10, 2);

                entity.Property(e => e.DataRegistro)
                      .IsRequired();
            });

            modelBuilder.Entity<Alerta>(entity =>
            {
                entity.ToTable("Alertas");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Mensagem)
                      .IsRequired()
                      .HasMaxLength(250);

                entity.Property(e => e.DataCriacao).IsRequired();
                entity.Property(e => e.Resolvido).IsRequired();
            });
        }
    }
}
