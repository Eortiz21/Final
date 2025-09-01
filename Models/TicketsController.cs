using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Primera.Models
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
            var applicationDbContext = _context.Tickets.Include(t => t.EspacioEstacionamiento).Include(t => t.Tarifa).Include(t => t.Vehiculo);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.EspacioEstacionamiento)
                .Include(t => t.Tarifa)
                .Include(t => t.Vehiculo)
                .FirstOrDefaultAsync(m => m.Id_Ticket == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            ViewData["Id_Espacio"] = new SelectList(_context.EspacioEstacionamientos, "Id_Espacio", "Estado");
            ViewData["Id_Tarifa"] = new SelectList(_context.Tarifas, "Id_Tarifa", "Descripcion");
            ViewData["NoPlaca"] = new SelectList(_context.Vehiculos, "NoPlaca", "NoPlaca");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id_Ticket,NoPlaca,Id_Espacio,Fecha_hora_entrada,Fecha_hora_salida,Id_Tarifa,PagoTotal")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Id_Espacio"] = new SelectList(_context.EspacioEstacionamientos, "Id_Espacio", "Estado", ticket.Id_Espacio);
            ViewData["Id_Tarifa"] = new SelectList(_context.Tarifas, "Id_Tarifa", "Descripcion", ticket.Id_Tarifa);
            ViewData["NoPlaca"] = new SelectList(_context.Vehiculos, "NoPlaca", "NoPlaca", ticket.NoPlaca);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["Id_Espacio"] = new SelectList(_context.EspacioEstacionamientos, "Id_Espacio", "Estado", ticket.Id_Espacio);
            ViewData["Id_Tarifa"] = new SelectList(_context.Tarifas, "Id_Tarifa", "Descripcion", ticket.Id_Tarifa);
            ViewData["NoPlaca"] = new SelectList(_context.Vehiculos, "NoPlaca", "NoPlaca", ticket.NoPlaca);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id_Ticket,NoPlaca,Id_Espacio,Fecha_hora_entrada,Fecha_hora_salida,Id_Tarifa,PagoTotal")] Ticket ticket)
        {
            if (id != ticket.Id_Ticket)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id_Ticket))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Id_Espacio"] = new SelectList(_context.EspacioEstacionamientos, "Id_Espacio", "Estado", ticket.Id_Espacio);
            ViewData["Id_Tarifa"] = new SelectList(_context.Tarifas, "Id_Tarifa", "Descripcion", ticket.Id_Tarifa);
            ViewData["NoPlaca"] = new SelectList(_context.Vehiculos, "NoPlaca", "NoPlaca", ticket.NoPlaca);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.EspacioEstacionamiento)
                .Include(t => t.Tarifa)
                .Include(t => t.Vehiculo)
                .FirstOrDefaultAsync(m => m.Id_Ticket == id);
            if (ticket == null)
            {
                return NotFound();
            }

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
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id_Ticket == id);
        }
    }
}
