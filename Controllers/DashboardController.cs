using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Primera.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

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
            // KPIs básicos
            var totalVehiculos = await _context.Vehiculos.CountAsync();
            var totalClientes = await _context.Clientes.CountAsync();
            var totalEspacios = await _context.EspacioEstacionamientos.CountAsync();
            var ocupados = await _context.EspacioEstacionamientos.CountAsync(e => e.Estado == "Ocupado");
            var disponibles = totalEspacios - ocupados;

            // Últimos 5 vehículos registrados
            var ultimosVehiculos = await _context.Vehiculos
                .Include(v => v.TipoVehiculo)
                .Include(v => v.Cliente)
                .OrderByDescending(v => v.NoPlaca)
                .Take(5)
                .ToListAsync();

            // Vehículos por tipo
            var vehiculosPorTipo = await _context.Vehiculos
                .Include(v => v.TipoVehiculo)
                .GroupBy(v => v.TipoVehiculo.Descripcion)
                .Select(g => new { Tipo = g.Key, Cantidad = g.Count() })
                .ToListAsync();

            // Vehículo más parqueado
            var vehiculoMasParqueado = await _context.Tickets
                .Include(t => t.Vehiculo)
                .ThenInclude(v => v.TipoVehiculo)
                .GroupBy(t => t.Vehiculo.TipoVehiculo.Descripcion)
                .Select(g => new { Tipo = g.Key, Cantidad = g.Count() })
                .OrderByDescending(g => g.Cantidad)
                .FirstOrDefaultAsync();

            // Tickets por estado
            var ticketsEnProgreso = await _context.Tickets.CountAsync(t => t.Estado == "En Progreso");
            var ticketsCerrados = await _context.Tickets.CountAsync(t => t.Estado == "Cerrado");

            // Clientes por día últimos 7 días
            var fechaInicio = DateTime.Now.AddDays(-6).Date;
            var clientesPorDia = await _context.Tickets
                .Where(t => t.Fecha_hora_entrada.Date >= fechaInicio)
                .GroupBy(t => t.Fecha_hora_entrada.Date)
                .Select(g => new
                {
                    Dia = g.Key,
                    Cantidad = g.Select(t => t.Vehiculo.Id_Cliente).Distinct().Count()
                })
                .ToListAsync();

            var dias = Enumerable.Range(0, 7)
                .Select(i => fechaInicio.AddDays(i).ToString("dd/MM"))
                .ToList();

            var clientesPorDiaData = dias
                .Select(d => clientesPorDia.FirstOrDefault(c => c.Dia.ToString("dd/MM") == d)?.Cantidad ?? 0)
                .ToList();

            // Dinero ganado por día últimos 7 días
            var pagosPorDia = await _context.Pagos
                .Where(p => p.FechaPago.Date >= fechaInicio && p.EstadoPago == "Pagado")
                .GroupBy(p => p.FechaPago.Date)
                .Select(g => new { Dia = g.Key, Total = g.Sum(p => p.MontoPago) })
                .ToListAsync();

            var ingresosPorDia = dias
                .Select(d => pagosPorDia.FirstOrDefault(p => p.Dia.ToString("dd/MM") == d)?.Total ?? 0)
                .ToList();

            // NUEVOS KPIs
            var dineroTotal = await _context.Pagos
                .Where(p => p.EstadoPago == "Pagado")
                .SumAsync(p => p.MontoPago);

            var ticketsPagadosHoy = await _context.Pagos
                .Where(p => p.FechaPago.Date == DateTime.Now.Date && p.EstadoPago == "Pagado")
                .CountAsync();

            // Guardar en ViewData
            ViewData["TotalVehiculos"] = totalVehiculos;
            ViewData["TotalClientes"] = totalClientes;
            ViewData["TotalEspacios"] = totalEspacios;
            ViewData["Ocupados"] = ocupados;
            ViewData["Disponibles"] = disponibles;
            ViewData["UltimosVehiculos"] = ultimosVehiculos;

            ViewData["VehiculosPorTipoLabels"] = vehiculosPorTipo.Select(v => v.Tipo).ToList();
            ViewData["VehiculosPorTipoData"] = vehiculosPorTipo.Select(v => v.Cantidad).ToList();

            ViewData["VehiculoMasParqueado"] = vehiculoMasParqueado?.Tipo ?? "N/A";

            ViewData["TicketsEnProgreso"] = ticketsEnProgreso;
            ViewData["TicketsCerrados"] = ticketsCerrados;

            ViewData["Dias"] = dias;
            ViewData["ClientesPorDiaData"] = clientesPorDiaData;
            ViewData["IngresosPorDia"] = ingresosPorDia;

            // NUEVOS KPIs
            ViewData["DineroTotal"] = dineroTotal;
            ViewData["TicketsPagadosHoy"] = ticketsPagadosHoy;

            return View();
        }
    }
}
