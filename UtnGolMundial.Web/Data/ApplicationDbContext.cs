using Microsoft.EntityFrameworkCore;
using UtnGolMundial.Web.Models;

namespace UtnGolMundial.Web.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Billetera> Billeteras { get; set; }
    public DbSet<Transaccion> Transacciones { get; set; }
    public DbSet<Partido> Partidos { get; set; }
    public DbSet<Prediccion> Predicciones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

   
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Correo)
            .IsUnique();
    }
}