using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Primera.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Primera.Controllers
{
    public class ClientesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Clientes
        public async Task<IActionResult> Index()
        {
            var clientes = await _context.Clientes.ToListAsync();
            return View(clientes);
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Clientes.FirstOrDefaultAsync(m => m.Id_Cliente == id);
            if (cliente == null) return NotFound();

            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            if (_context.Clientes.Any(c => c.NumeroDocumentacion == cliente.NumeroDocumentacion))
            {
                ModelState.AddModelError("NumeroDocumentacion", "Este número de documento ya está registrado.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(cliente);
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            return View(cliente);
        }

        // POST: Clientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cliente cliente)
        {
            if (id != cliente.Id_Cliente)
                return NotFound();

            // Validar duplicado SOLO si cambió el documento
            var existente = await _context.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id_Cliente == id);

            if (existente == null)
                return NotFound();

            if (existente.NumeroDocumentacion != cliente.NumeroDocumentacion)
            {
                if (_context.Clientes.Any(c => c.NumeroDocumentacion == cliente.NumeroDocumentacion))
                {
                    ModelState.AddModelError("NumeroDocumentacion", "Este número de documento ya está registrado.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Clientes.Any(e => e.Id_Cliente == id))
                        return NotFound();
                    else
                        throw;
                }
            }

            return View(cliente);
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Clientes.FirstOrDefaultAsync(m => m.Id_Cliente == id);
            if (cliente == null) return NotFound();

            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Validación AJAX — se usa en Create y Edit (evita duplicados)
        [AcceptVerbs("GET", "POST")]
        public IActionResult VerificarDocumento(string numeroDocumentacion, int id_Cliente)
        {
            bool existe = _context.Clientes
                .Any(c => c.NumeroDocumentacion == numeroDocumentacion && c.Id_Cliente != id_Cliente);

            if (existe)
                return Json($"El número de documento {numeroDocumentacion} ya existe.");

            return Json(true);
        }
    }
}
