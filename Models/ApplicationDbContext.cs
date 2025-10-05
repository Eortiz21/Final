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

            // Tablas
            modelBuilder.Entity<Vehiculo>().ToTable("Vehiculos");
            modelBuilder.Entity<Cliente>().ToTable("Clientes");
            modelBuilder.Entity<TipoVehiculo>().ToTable("TipoVehiculos");

            // Relaciones Vehiculo → Cliente
            modelBuilder.Entity<Vehiculo>()
                .HasOne(v => v.Cliente)
                .WithMany(c => c.Vehiculos)
                .HasForeignKey(v => v.Id_Cliente)
                .OnDelete(DeleteBehavior.Cascade);

            // Relaciones Vehiculo → TipoVehiculo
            modelBuilder.Entity<Vehiculo>()
                .HasOne(v => v.TipoVehiculo)
                .WithMany(t => t.Vehiculos)
                .HasForeignKey(v => v.Id_Tipo)
                .OnDelete(DeleteBehavior.Cascade);

            // Relaciones Ticket → Vehiculo
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Vehiculo)
                .WithMany()
                .HasForeignKey(t => t.NoPlaca)      // usa columna existente NoPlaca
                .HasPrincipalKey(v => v.NoPlaca);

            // Relaciones Ticket → EspacioEstacionamiento
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.EspacioEstacionamiento)
                .WithMany()
                .HasForeignKey(t => t.Id_Espacio)
                .HasPrincipalKey(e => e.Id_Espacio);

            // Relaciones Ticket → Tarifa
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Tarifa)
                .WithMany()
                .HasForeignKey(t => t.Id_Tarifa)
                .HasPrincipalKey(tr => tr.Id_Tarifa);
        }
    }
}
