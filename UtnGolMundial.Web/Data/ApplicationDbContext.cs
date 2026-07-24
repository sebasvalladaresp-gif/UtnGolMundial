using Microsoft.EntityFrameworkCore;
using UtnGolMundial.Web.Models;

namespace UtnGolMundial.Web.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Billetera> Billeteras => Set<Billetera>();
    public DbSet<Transaccion> Transacciones => Set<Transaccion>();
    public DbSet<Partido> Partidos => Set<Partido>();
    public DbSet<Prediccion> Predicciones => Set<Prediccion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Correo).IsUnique();
        });

        // Billetera: 1 a 1 con Usuario (UsuarioId es PK y FK a la vez)
        modelBuilder.Entity<Billetera>(entity =>
        {
            entity.HasKey(b => b.UsuarioId);
            entity.HasOne<Usuario>()
                  .WithOne()
                  .HasForeignKey<Billetera>(b => b.UsuarioId);
        });

        // Transaccion: N a 1 con Usuario
        modelBuilder.Entity<Transaccion>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.HasOne<Usuario>()
                  .WithMany()
                  .HasForeignKey(t => t.UsuarioId);
        });

        // Partido
        modelBuilder.Entity<Partido>(entity =>
        {
            entity.HasKey(p => p.Id);
        });

        // Prediccion: N a 1 con Usuario, N a 1 con Partido
        modelBuilder.Entity<Prediccion>(entity =>
        {
            entity.HasKey(pr => pr.Id);
            entity.HasOne<Usuario>()
                  .WithMany()
                  .HasForeignKey(pr => pr.UsuarioId);
            entity.HasOne<Partido>()
                  .WithMany()
                  .HasForeignKey(pr => pr.PartidoId);
        });
    }
}