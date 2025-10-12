using System;
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
            var pagos = _context.Pagos
                .Include(p => p.Ticket)
                .ThenInclude(t => t.Tarifa);

            return View(await pagos.ToListAsync());
        }

        // GET: Pagoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var pago = await _context.Pagos
                .Include(p => p.Ticket)
                .ThenInclude(t => t.Tarifa)
                .FirstOrDefaultAsync(m => m.Id_Pago == id);

            if (pago == null) return NotFound();

            return View(pago);
        }

        // GET: Pagoes/Create
        public IActionResult Create()
        {
            var ticketsEnProgreso = _context.Tickets
                .Include(t => t.Tarifa)
                .Where(t => t.Estado == "En Progreso")
                .ToList();

            ViewData["Id_Ticket"] = new SelectList(
                ticketsEnProgreso,
                "Id_Ticket",
                "NoPlaca"
            );

            // Serializar los datos necesarios para cálculo en cliente
            ViewBag.TicketsJson = System.Text.Json.JsonSerializer.Serialize(
                ticketsEnProgreso.Select(t => new
                {
                    id = t.Id_Ticket,
                    entrada = t.Fecha_hora_entrada,
                    monto = t.Tarifa.Monto
                })
            );

            return View(new Pago());
        }

        // POST: Pagoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id_Ticket,FechaPago,MetodoPago,EstadoPago,MontoPago")] Pago pago, string accion)
        {
            var ticket = _context.Tickets
                .Include(t => t.Tarifa)
                .FirstOrDefault(t => t.Id_Ticket == pago.Id_Ticket);

            if (ticket == null)
            {
                ModelState.AddModelError("Id_Ticket", "Debe seleccionar un ticket válido.");

                // Volver a cargar SelectList y Json para la vista
                var ticketsEnProgreso = _context.Tickets
                    .Include(t => t.Tarifa)
                    .Where(t => t.Estado == "En Progreso")
                    .ToList();

                ViewData["Id_Ticket"] = new SelectList(
                    ticketsEnProgreso,
                    "Id_Ticket",
                    "NoPlaca",
                    pago.Id_Ticket
                );

                ViewBag.TicketsJson = System.Text.Json.JsonSerializer.Serialize(
                    ticketsEnProgreso.Select(t => new
                    {
                        id = t.Id_Ticket,
                        entrada = t.Fecha_hora_entrada,
                        monto = t.Tarifa.Monto
                    })
                );

                return View(pago);
            }

            pago.Ticket = ticket;

            if (accion == "calcular")
            {
                try
                {
                    DateTime entrada = ticket.Fecha_hora_entrada;
                    DateTime salida = pago.FechaPago;

                    double horas = (salida - entrada).TotalHours;
                    if (horas < 0) horas = 0;

                    int horasRedondeadas = (int)Math.Ceiling(horas);
                    pago.MontoPago = horasRedondeadas * ticket.Tarifa.Monto;

                    TempData["MensajeExito"] = $"Monto calculado: {pago.MontoPago:C}";

                    ModelState.Clear();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }

                // Recargar SelectList y Json después del cálculo
                var ticketsEnProgreso = _context.Tickets
                    .Include(t => t.Tarifa)
                    .Where(t => t.Estado == "En Progreso")
                    .ToList();

                ViewData["Id_Ticket"] = new SelectList(
                    ticketsEnProgreso,
                    "Id_Ticket",
                    "NoPlaca",
                    pago.Id_Ticket
                );

                ViewBag.TicketsJson = System.Text.Json.JsonSerializer.Serialize(
                    ticketsEnProgreso.Select(t => new
                    {
                        id = t.Id_Ticket,
                        entrada = t.Fecha_hora_entrada,
                        monto = t.Tarifa.Monto
                    })
                );

                return View(pago);
            }

            if (accion == "guardar" && ModelState.IsValid)
            {
                DateTime entradaFinal = ticket.Fecha_hora_entrada;
                DateTime salidaFinal = pago.FechaPago;

                double horasFinal = (salidaFinal - entradaFinal).TotalHours;
                if (horasFinal < 0) horasFinal = 0;

                int horasRedondeadasFinal = (int)Math.Ceiling(horasFinal);
                pago.MontoPago = horasRedondeadasFinal * ticket.Tarifa.Monto;

                _context.Pagos.Add(pago);

                // Actualizar ticket a "Cerrado"
                var ticketDb = await _context.Tickets.FindAsync(ticket.Id_Ticket);
                if (ticketDb != null)
                {
                    ticketDb.Estado = "Cerrado";
                    _context.Update(ticketDb);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Si no es válido o falla algo, recargar dropdown y JSON
            {
                var ticketsEnProgreso = _context.Tickets
                    .Include(t => t.Tarifa)
                    .Where(t => t.Estado == "En Progreso")
                    .ToList();

                ViewData["Id_Ticket"] = new SelectList(
                    ticketsEnProgreso,
                    "Id_Ticket",
                    "NoPlaca",
                    pago.Id_Ticket
                );

                ViewBag.TicketsJson = System.Text.Json.JsonSerializer.Serialize(
                    ticketsEnProgreso.Select(t => new
                    {
                        id = t.Id_Ticket,
                        entrada = t.Fecha_hora_entrada,
                        monto = t.Tarifa.Monto
                    })
                );
            }

            return View(pago);
        }

        // GET: Pagoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var pago = await _context.Pagos.FindAsync(id);
            if (pago == null) return NotFound();

            ViewData["Id_Ticket"] = new SelectList(
                _context.Tickets.Include(t => t.Tarifa),
                "Id_Ticket",
                "NoPlaca",
                pago.Id_Ticket
            );

            return View(pago);
        }

        // POST: Pagoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id_Pago,Id_Ticket,FechaPago,MontoPago,MetodoPago,EstadoPago")] Pago pago)
        {
            if (id != pago.Id_Pago) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var ticket = _context.Tickets.Include(t => t.Tarifa).FirstOrDefault(t => t.Id_Ticket == pago.Id_Ticket);
                    if (ticket != null)
                    {
                        pago.Ticket = ticket;

                        DateTime entrada = ticket.Fecha_hora_entrada;
                        DateTime salida = pago.FechaPago;

                        double horas = (salida - entrada).TotalHours;
                        if (horas < 0) horas = 0;

                        int horasRedondeadas = (int)Math.Ceiling(horas);
                        pago.MontoPago = horasRedondeadas * ticket.Tarifa.Monto;

                        // Actualizar ticket a cerrado si no lo está
                        var ticketDb = await _context.Tickets.FindAsync(ticket.Id_Ticket);
                        if (ticketDb != null)
                        {
                            ticketDb.Estado = "Cerrado";
                            _context.Update(ticketDb);
                        }
                    }

                    _context.Update(pago);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Pagos.Any(e => e.Id_Pago == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["Id_Ticket"] = new SelectList(
                _context.Tickets.Include(t => t.Tarifa),
                "Id_Ticket",
                "NoPlaca",
                pago.Id_Ticket
            );

            return View(pago);
        }

        // GET: Pagoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var pago = await _context.Pagos
                .Include(p => p.Ticket)
                .ThenInclude(t => t.Tarifa)
                .FirstOrDefaultAsync(m => m.Id_Pago == id);

            if (pago == null) return NotFound();

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
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
