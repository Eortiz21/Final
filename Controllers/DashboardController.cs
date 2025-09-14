using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Primera.Models;

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
            // Datos principales para el Dashboard
            var totalVehiculos = await _context.Vehiculos.CountAsync();
            var totalClientes = await _context.Clientes.CountAsync();
            var totalEspacios = await _context.EspacioEstacionamientos.CountAsync();
            var totalTickets = await _context.Tickets.CountAsync();
            var totalPagos = await _context.Pagos.CountAsync();

            // Últimos 5 vehículos registrados
            var ultimosVehiculos = await _context.Vehiculos
                .OrderByDescending(v => v.NoPlaca) // si tienes FechaRegistro, cámbialo aquí
                .Take(5)
                .ToListAsync();

            // Pasamos datos a la vista
            ViewData["TotalVehiculos"] = totalVehiculos;
            ViewData["TotalClientes"] = totalClientes;
            ViewData["TotalEspacios"] = totalEspacios;
            ViewData["TotalTickets"] = totalTickets;
            ViewData["TotalPagos"] = totalPagos;
            ViewData["UltimosVehiculos"] = ultimosVehiculos;

            return View();
        }
    }
}
