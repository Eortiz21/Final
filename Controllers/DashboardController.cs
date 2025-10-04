using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Primera.Models;
using System.Linq;

namespace Primera.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Datos principales
            var totalVehiculos = await _context.Vehiculos.CountAsync();
            var totalClientes = await _context.Clientes.CountAsync();
            var totalEspacios = await _context.EspacioEstacionamientos.CountAsync();
            var ocupados = await _context.EspacioEstacionamientos.CountAsync(e => e.Estado == "Ocupado");
            var disponibles = totalEspacios - ocupados;

            // Últimos 5 vehículos registrados (ordenados por placa de ejemplo)
            var ultimosVehiculos = await _context.Vehiculos
                .OrderByDescending(v => v.NoPlaca)
                .Take(5)
                .Include(v => v.Cliente)
                .Include(v => v.TipoVehiculo)
                .ToListAsync();

            // Vehículos agrupados por tipo
            var vehiculosPorTipo = await _context.Vehiculos
                .Include(v => v.TipoVehiculo)
                .GroupBy(v => v.TipoVehiculo.Descripcion) // 👈 NombreTipo debe existir en tu modelo TipoVehiculo
                .Select(g => new { Tipo = g.Key, Cantidad = g.Count() })
                .ToListAsync();

            ViewData["VehiculosPorTipoLabels"] = vehiculosPorTipo.Select(v => v.Tipo).ToList();
            ViewData["VehiculosPorTipoData"] = vehiculosPorTipo.Select(v => v.Cantidad).ToList();

            // Ocupación
            ViewData["Ocupados"] = ocupados;
            ViewData["Disponibles"] = disponibles;

            // Ingresos mensuales (últimos 6 meses)
            var ingresos = await _context.Pagos
                .GroupBy(p => new { p.FechaPago.Year, p.FechaPago.Month })
                .Select(g => new {
                    Mes = g.Key.Month,
                    Anio = g.Key.Year,
                    Total = g.Sum(p => p.MontoPago) // 👈 ahora usa MontoPago
                })
                .OrderBy(g => g.Anio).ThenBy(g => g.Mes)
                .Take(6)
                .ToListAsync();

            var meses = ingresos.Select(i => $"{i.Mes}/{i.Anio}").ToList();
            var valores = ingresos.Select(i => i.Total).ToList();

            ViewData["MesesIngresos"] = meses;
            ViewData["IngresosMensuales"] = valores;

            // Datos para tarjetas y listas
            ViewData["TotalVehiculos"] = totalVehiculos;
            ViewData["TotalClientes"] = totalClientes;
            ViewData["TotalEspacios"] = totalEspacios;
            ViewData["UltimosVehiculos"] = ultimosVehiculos ?? new List<Vehiculo>();

            return View();
        }
    }
}
