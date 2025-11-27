using Microsoft.EntityFrameworkCore;

namespace EFEnergiaAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets serão adicionados aqui conforme os modelos forem criados
    // Exemplo:
    // public DbSet<Setor> Setores { get; set; }
    // public DbSet<Equipamento> Equipamentos { get; set; }
    // public DbSet<Leitura> Leituras { get; set; }
    // public DbSet<Alerta> Alertas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurações de modelos serão adicionadas aqui
        // Exemplo:
        // modelBuilder.ApplyConfiguration(new SetorConfiguration());
    }
}

