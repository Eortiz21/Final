using Primera.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Primera.Models;
using System.Drawing;
using System.Drawing.Printing;
using iTextSharp.text;
using iTextSharp.text.pdf;

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

                // Cerrar ticket y liberar espacio
                var ticketDb = await _context.Tickets.FindAsync(ticket.Id_Ticket);
                if (ticketDb != null)
                {
                    ticketDb.Estado = "Cerrado";
                    _context.Update(ticketDb);

                    var espacio = await _context.EspacioEstacionamientos
                        .FirstOrDefaultAsync(e => e.Id_Espacio == ticketDb.Id_Espacio);

                    if (espacio != null)
                    {
                        espacio.Estado = "Libre";
                        _context.Update(espacio);
                    }
                }

                await _context.SaveChangesAsync();
                TempData["MensajeExito"] = "Pago realizado correctamente. El espacio ha sido liberado.";


                return RedirectToAction("Print", new { id = pago.Id_Pago });

            }

            var recargarTickets = _context.Tickets
                .Include(t => t.Tarifa)
                .Where(t => t.Estado == "En Progreso")
                .ToList();

            ViewData["Id_Ticket"] = new SelectList(
                recargarTickets,
                "Id_Ticket",
                "NoPlaca",
                pago.Id_Ticket
            );

            ViewBag.TicketsJson = System.Text.Json.JsonSerializer.Serialize(
                recargarTickets.Select(t => new
                {
                    id = t.Id_Ticket,
                    entrada = t.Fecha_hora_entrada,
                    monto = t.Tarifa.Monto
                })
            );

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

                        var ticketDb = await _context.Tickets.FindAsync(ticket.Id_Ticket);
                        if (ticketDb != null)
                        {
                            ticketDb.Estado = "Cerrado";
                            _context.Update(ticketDb);

                            var espacio = await _context.EspacioEstacionamientos
                                .FirstOrDefaultAsync(e => e.Id_Espacio == ticketDb.Id_Espacio);

                            if (espacio != null)
                            {
                                espacio.Estado = "Libre";
                                _context.Update(espacio);
                            }
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


        // GET: Pagoes/ReciboPartial/5
        public async Task<IActionResult> ReciboPartial(int? id)
        {
            if (id == null) return NotFound();

            var pago = await _context.Pagos
                .Include(p => p.Ticket)
                .ThenInclude(t => t.Tarifa)
                .Include(p => p.Ticket.Vehiculo)
                .Include(p => p.Ticket.EspacioEstacionamiento)
                .FirstOrDefaultAsync(p => p.Id_Pago == id);

            if (pago == null) return NotFound();

            return PartialView("_ReciboPartial", pago);
        }



        // GET: Pagoes/Print/5
        public async Task<IActionResult> Print(int id)
        {
            // Verificación básica de seguridad (ajusta a tu esquema de permisos)
            if (!User.Identity.IsAuthenticated)
            {
                return Forbid();
            }

            var pago = await _context.Pagos
                .Include(p => p.Ticket)
                    .ThenInclude(t => t.Vehiculo)
                .Include(p => p.Ticket)
                    .ThenInclude(t => t.EspacioEstacionamiento)
                .Include(p => p.Ticket)
                    .ThenInclude(t => t.Tarifa)
                .FirstOrDefaultAsync(p => p.Id_Pago == id);

            if (pago == null)
            {
                return NotFound();
            }

            // Devuelve la vista "Print" con el pago como modelo
            return View("Print", pago);
        }
        // Método para imprimir el ticket en la impresora predeterminada

        private string GenerarTicketTérmico(Pago pago)
        {
            var ticket = pago.Ticket;
            var espacio = ticket?.EspacioEstacionamiento;
            var tarifa = ticket?.Tarifa;

            string texto = "=== ESTACIONAMIENTO CENTRAL ===\n";
            texto += $"Fecha: {DateTime.Now}\n";
            texto += $"Placa: {ticket?.NoPlaca ?? "N/A"}\n";
            texto += $"Espacio: {espacio?.No_Espacio ?? "N/A"}\n";
            texto += $"Nivel: {espacio?.Nivel ?? "N/A"}\n";
            texto += $"Tarifa: Q{tarifa?.Monto ?? 0}\n";
            texto += $"Monto Total: Q{pago.MontoPago}\n";
            texto += $"Método: {pago.MetodoPago}\n";
            texto += $"--------------------------------\n";
            texto += "¡Gracias por su visita!\n";
            texto += "==============================\n";
            return texto;
        }
        private void ImprimirTicket(Pago pago)
        {
            string texto = GenerarTicketTérmico(pago);
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += (sender, e) =>
            {
                e.Graphics.DrawString(texto, new("Consolas", 10), Brushes.Black, new PointF(0, 0));
            };
            pd.Print();
        }


        // Nuevo método para reimprimir
        public async Task<IActionResult> Reimprimir(int id)
        {
            var pago = await _context.Pagos
                .Include(p => p.Ticket)
                .ThenInclude(t => t.EspacioEstacionamiento)
                .Include(p => p.Ticket.Tarifa)
                .FirstOrDefaultAsync(p => p.Id_Pago == id);

            if (pago == null)
                return NotFound();

            ImprimirTicket(pago);
            return RedirectToAction(nameof(Index));
        }

    }
}