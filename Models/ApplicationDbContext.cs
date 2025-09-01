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

    }
}
