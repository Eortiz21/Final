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
    public class PagoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PagoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Pagoes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Pagos.Include(p => p.Ticket);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Pagoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pago = await _context.Pagos
                .Include(p => p.Ticket)
                .FirstOrDefaultAsync(m => m.Id_Pago == id);
            if (pago == null)
            {
                return NotFound();
            }

            return View(pago);
        }

        // GET: Pagoes/Create
        public IActionResult Create()
        {
            ViewData["Id_Ticket"] = new SelectList(_context.Tickets, "Id_Ticket", "NoPlaca");
            return View();
        }

        // POST: Pagoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id_Pago,Id_Ticket,MontoPago,FechaPago,MetodoPago,EstadoPago")] Pago pago)
        {
            if (!ModelState.IsValid)
            {
                // Capturar errores de validación
                var errores = ModelState
                    .Where(ms => ms.Value.Errors.Any())
                    .Select(ms => new
                    {
                        Campo = ms.Key,
                        Errores = ms.Value.Errors.Select(e => e.ErrorMessage).ToList()
                    })
                    .ToList();

                // Mostrar en consola (para depurar)
                foreach (var err in errores)
                {
                    Console.WriteLine($"Campo con error: {err.Campo} → {string.Join(", ", err.Errores)}");
                }

                // Enviar errores a la vista
                TempData["ErroresValidacion"] = string.Join("<br>", errores.Select(e =>
                    $"<strong>{e.Campo}</strong>: {string.Join(", ", e.Errores)}"));
            }

            if (ModelState.IsValid)
            {
                _context.Add(pago);
                await _context.SaveChangesAsync();
                TempData["MensajeExito"] = "✅ Pago registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["Id_Ticket"] = new SelectList(_context.Tickets, "Id_Ticket", "NoPlaca", pago.Id_Ticket);
            return View(pago);
        }

        // GET: Pagoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pago = await _context.Pagos.FindAsync(id);
            if (pago == null)
            {
                return NotFound();
            }
            ViewData["Id_Ticket"] = new SelectList(_context.Tickets, "Id_Ticket", "NoPlaca", pago.Id_Ticket);
            return View(pago);
        }

        // POST: Pagoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id_Pago,Id_Ticket,MontoPago,FechaPago,MetodoPago,EstadoPago")] Pago pago)
        {
            if (id != pago.Id_Pago)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var errores = ModelState
                    .Where(ms => ms.Value.Errors.Any())
                    .Select(ms => new
                    {
                        Campo = ms.Key,
                        Errores = ms.Value.Errors.Select(e => e.ErrorMessage).ToList()
                    })
                    .ToList();

                foreach (var err in errores)
                {
                    Console.WriteLine($"Campo con error: {err.Campo} → {string.Join(", ", err.Errores)}");
                }

                TempData["ErroresValidacion"] = string.Join("<br>", errores.Select(e =>
                    $"<strong>{e.Campo}</strong>: {string.Join(", ", e.Errores)}"));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pago);
                    await _context.SaveChangesAsync();
                    TempData["MensajeExito"] = "✅ Pago actualizado correctamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PagoExists(pago.Id_Pago))
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
            ViewData["Id_Ticket"] = new SelectList(_context.Tickets, "Id_Ticket", "NoPlaca", pago.Id_Ticket);
            return View(pago);
        }

        // GET: Pagoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pago = await _context.Pagos
                .Include(p => p.Ticket)
                .FirstOrDefaultAsync(m => m.Id_Pago == id);
            if (pago == null)
            {
                return NotFound();
            }

            return View(pago);
        }

        // POST: Pagoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pago = await _context.Pagos.FindAsync(id);
            if (pago != null)
            {
                _context.Pagos.Remove(pago);
            }

            await _context.SaveChangesAsync();
            TempData["MensajeExito"] = "🗑️ Pago eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        private bool PagoExists(int id)
        {
            return _context.Pagos.Any(e => e.Id_Pago == id);
        }
    }
}
