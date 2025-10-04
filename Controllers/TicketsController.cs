using System;
using System.Collections.Generic;
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
            var applicationDbContext = _context.Tickets
                .Include(t => t.EspacioEstacionamiento)
                .Include(t => t.Tarifa)
                .Include(t => t.Vehiculo);

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

            // Agregar ViewBag.Tarifas para tu foreach en la vista
            ViewBag.Tarifas = _context.Tarifas.Select(t => new
            {
                t.Id_Tarifa,
                Nombre = t.Descripcion,
                Precio = t.Monto,
                Tipo = t.TipoTarifa
            }).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NoPlaca,Id_Espacio,Fecha_hora_entrada,Fecha_hora_salida,Id_Tarifa,PagoTotal")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Recargar los combos si falla la validación
            ViewData["Id_Espacio"] = new SelectList(_context.EspacioEstacionamientos, "Id_Espacio", "Estado", ticket.Id_Espacio);
            ViewData["Id_Tarifa"] = new SelectList(_context.Tarifas, "Id_Tarifa", "Descripcion", ticket.Id_Tarifa);
            ViewData["NoPlaca"] = new SelectList(_context.Vehiculos, "NoPlaca", "NoPlaca", ticket.NoPlaca);

            // Recargar ViewBag.Tarifas para el foreach de la vista
            ViewBag.Tarifas = _context.Tarifas.Select(t => new
            {
                t.Id_Tarifa,
                Nombre = t.Descripcion,
                Precio = t.Monto,
                Tipo = t.TipoTarifa
            }).ToList();

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id_Ticket,NoPlaca,Id_Espacio,Fecha_hora_entrada,Fecha_hora_salida,Id_Tarifa")] Ticket ticket)
        {
            if (id != ticket.Id_Ticket)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 🔹 Recalcular PagoTotal en edición
                    ticket.PagoTotal = CalcularPago(ticket);

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

        // 🔹 Método privado para calcular el pago
        private decimal CalcularPago(Ticket ticket)
        {
            // Buscar la tarifa seleccionada
            var tarifa = _context.Tarifas.FirstOrDefault(t => t.Id_Tarifa == ticket.Id_Tarifa);

            // Si no hay tarifa o no hay fecha de salida, devolver 0
            if (tarifa == null || ticket.Fecha_hora_salida == null)
                return 0;

            // Asegurarse de que Fecha_hora_entrada tenga valor válido
            if (ticket.Fecha_hora_entrada == DateTime.MinValue)
            {
                return 0;
            }

            // Calcular la diferencia de tiempo
            TimeSpan tiempo = TimeSpan.Zero;

            if (ticket.Fecha_hora_salida.HasValue)
            {
                tiempo = ticket.Fecha_hora_salida.Value - ticket.Fecha_hora_entrada;
            }

            if (tiempo.TotalMinutes <= 0)
                return 0;

            decimal total = 0;

            // Usar ?.ToLower() para evitar null en TipoTarifa
            string tipo = tarifa.TipoTarifa?.ToLower() ?? "";

            switch (tipo)
            {
                case "hora":
                    total = (decimal)Math.Ceiling(tiempo.TotalHours) * tarifa.Monto;
                    break;
                case "dia":
                    total = (decimal)Math.Ceiling(tiempo.TotalDays) * tarifa.Monto;
                    break;
                case "semana":
                    total = (decimal)Math.Ceiling(tiempo.TotalDays / 7) * tarifa.Monto;
                    break;
                default:
                    // Si TipoTarifa no está definido, cobrar el monto base
                    total = tarifa.Monto;
                    break;
            }

            return total;
        }
    }
}
