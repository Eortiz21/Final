using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Primera.Models;

namespace Primera.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            var tickets = _context.Tickets
                .Include(t => t.Vehiculo)
                .Include(t => t.EspacioEstacionamiento)
                .Include(t => t.Tarifa);

            return View(await tickets.ToListAsync());
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.Tickets
                .Include(t => t.Vehiculo)
                .Include(t => t.EspacioEstacionamiento)
                .Include(t => t.Tarifa)
                .FirstOrDefaultAsync(t => t.Id_Ticket == id);

            if (ticket == null) return NotFound();

            return View(ticket);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            ViewData["NoPlaca"] = new SelectList(
                _context.Vehiculos
                    .Where(v => v.Estado == "En parqueo")
                    .Select(v => new { v.NoPlaca }),
                "NoPlaca",
                "NoPlaca"
            );

            ViewData["Id_Espacio"] = new SelectList(
                _context.EspacioEstacionamientos
                    .Where(e => e.Estado == "Libre")
                    .Select(e => new
                    {
                        e.Id_Espacio,
                        Display = $"{e.No_Espacio} - Nivel {e.Nivel} - {e.TipoEspacio}"
                    }),
                "Id_Espacio",
                "Display"
            );

            ViewData["Id_Tarifa"] = new SelectList(_context.Tarifas, "Id_Tarifa", "TipoTarifa");
            ViewData["Estado"] = new SelectList(new[] { "En Progreso", "Cerrado" });

            return View();
        }

        // POST: Tickets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NoPlaca,Id_Espacio,Fecha_hora_entrada,Id_Tarifa,Estado")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                // Cambiar estado del vehículo a "Fuera de parqueo"
                var vehiculo = await _context.Vehiculos.FindAsync(ticket.NoPlaca);
                if (vehiculo != null)
                {
                    vehiculo.Estado = "Fuera de parqueo";
                    _context.Vehiculos.Update(vehiculo);
                }

                // Cambiar estado del espacio a "Ocupado"
                var espacio = await _context.EspacioEstacionamientos.FindAsync(ticket.Id_Espacio);
                if (espacio != null)
                {
                    espacio.Estado = "Ocupado";
                    _context.EspacioEstacionamientos.Update(espacio);
                }

                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Recargar dropdowns si hay error
            ViewData["NoPlaca"] = new SelectList(
                _context.Vehiculos.Where(v => v.Estado == "En parqueo").Select(v => new { v.NoPlaca }),
                "NoPlaca",
                "NoPlaca",
                ticket.NoPlaca
            );

            ViewData["Id_Espacio"] = new SelectList(
                _context.EspacioEstacionamientos.Where(e => e.Estado == "Libre").Select(e => new
                {
                    e.Id_Espacio,
                    Display = $"{e.No_Espacio} - Nivel {e.Nivel} - {e.TipoEspacio}"
                }),
                "Id_Espacio",
                "Display",
                ticket.Id_Espacio
            );

            ViewData["Id_Tarifa"] = new SelectList(_context.Tarifas, "Id_Tarifa", "TipoTarifa", ticket.Id_Tarifa);
            ViewData["Estado"] = new SelectList(new[] { "En Progreso", "Cerrado" }, ticket.Estado);

            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            ViewData["NoPlaca"] = new SelectList(
                _context.Vehiculos.Select(v => new { v.NoPlaca }),
                "NoPlaca",
                "NoPlaca",
                ticket.NoPlaca
            );

            ViewData["Id_Espacio"] = new SelectList(
                _context.EspacioEstacionamientos
                    .Select(e => new
                    {
                        e.Id_Espacio,
                        Display = $"{e.No_Espacio} - Nivel {e.Nivel} - {e.TipoEspacio}"
                    }),
                "Id_Espacio",
                "Display",
                ticket.Id_Espacio
            );

            ViewData["Id_Tarifa"] = new SelectList(_context.Tarifas, "Id_Tarifa", "TipoTarifa", ticket.Id_Tarifa);
            ViewData["Estado"] = new SelectList(new[] { "En Progreso", "Cerrado" }, ticket.Estado);

            return View(ticket);
        }

        // POST: Tickets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id_Ticket,NoPlaca,Id_Espacio,Fecha_hora_entrada,Id_Tarifa,Estado")] Ticket ticket)
        {
            if (id != ticket.Id_Ticket) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id_Ticket)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["NoPlaca"] = new SelectList(_context.Vehiculos.Select(v => new { v.NoPlaca }), "NoPlaca", "NoPlaca", ticket.NoPlaca);
            ViewData["Id_Espacio"] = new SelectList(
                _context.EspacioEstacionamientos.Select(e => new
                {
                    e.Id_Espacio,
                    Display = $"{e.No_Espacio} - Nivel {e.Nivel} - {e.TipoEspacio}"
                }),
                "Id_Espacio",
                "Display",
                ticket.Id_Espacio
            );

            ViewData["Id_Tarifa"] = new SelectList(_context.Tarifas, "Id_Tarifa", "TipoTarifa", ticket.Id_Tarifa);
            ViewData["Estado"] = new SelectList(new[] { "En Progreso", "Cerrado" }, ticket.Estado);

            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.Tickets
                .Include(t => t.Vehiculo)
                .Include(t => t.EspacioEstacionamiento)
                .Include(t => t.Tarifa)
                .FirstOrDefaultAsync(t => t.Id_Ticket == id);

            if (ticket == null) return NotFound();

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id_Ticket == id);
        }
    }
}
