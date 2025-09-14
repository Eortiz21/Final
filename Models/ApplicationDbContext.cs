using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Primera.Models
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Cliente> Clientes { get; set; } = default!;
        public DbSet<Vehiculo> Vehiculos { get; set; } = default!;
        public DbSet<EspacioEstacionamiento> EspacioEstacionamientos { get; set; } = default!;
        public DbSet<Tarifa> Tarifas { get; set; } = default!;
        public DbSet<Ticket> Tickets { get; set; } = default!;
        public DbSet<Pago> Pagos { get; set; } = default!;
        public DbSet<TipoVehiculo> TipoVehiculos { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Vehiculo>().ToTable("Vehiculos");
            modelBuilder.Entity<Cliente>().ToTable("Clientes");
            modelBuilder.Entity<TipoVehiculo>().ToTable("TipoVehiculos");

            modelBuilder.Entity<Vehiculo>()
                .HasOne(v => v.Cliente)
                .WithMany(c => c.Vehiculos)
                .HasForeignKey(v => v.Id_Cliente)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Vehiculo>()
                .HasOne(v => v.TipoVehiculo)
                .WithMany(t => t.Vehiculos)
                .HasForeignKey(v => v.Id_Tipo)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

