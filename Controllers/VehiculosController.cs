using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Primera.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Primera.Controllers
{
    public class VehiculosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VehiculosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Vehiculos
        public async Task<IActionResult> Index()
        {
            var vehiculos = _context.Vehiculos
                .Include(v => v.Cliente)
                .Include(v => v.TipoVehiculo);

            return View(await vehiculos.ToListAsync());
        }

        // GET: Vehiculos/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var vehiculo = await _context.Vehiculos
                .Include(v => v.Cliente)
                .Include(v => v.TipoVehiculo)
                .FirstOrDefaultAsync(v => v.NoPlaca == id);

            if (vehiculo == null) return NotFound();

            return View(vehiculo);
        }

        // GET: Vehiculos/Create
        public IActionResult Create()
        {
            LoadSelectLists();
            return View();
        }

        // POST: Vehiculos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NoPlaca,Marca,Color,Id_Cliente,Id_Tipo,Estado")] Vehiculo vehiculo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Vehiculos.Add(vehiculo);

                    // Cambiar estado del cliente a "Registrado"
                    var cliente = await _context.Clientes.FindAsync(vehiculo.Id_Cliente);
                    if (cliente != null)
                    {
                        cliente.Estado = "Registrado";
                        _context.Clientes.Update(cliente);
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    ViewBag.Errores = new string[] { ex.InnerException?.Message ?? ex.Message };
                }
            }

            LoadSelectLists(vehiculo.Id_Cliente, vehiculo.Id_Tipo);
            return View(vehiculo);
        }

        // GET: Vehiculos/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null) return NotFound();

            LoadSelectLists(vehiculo.Id_Cliente, vehiculo.Id_Tipo);
            return View(vehiculo);
        }

        // POST: Vehiculos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("NoPlaca,Marca,Color,Id_Cliente,Id_Tipo,Estado")] Vehiculo vehiculo)
        {
            if (id != vehiculo.NoPlaca) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vehiculo);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehiculoExists(vehiculo.NoPlaca)) return NotFound();
                    else throw;
                }
                catch (DbUpdateException ex)
                {
                    ViewBag.Errores = new string[] { ex.InnerException?.Message ?? ex.Message };
                }
            }

            LoadSelectLists(vehiculo.Id_Cliente, vehiculo.Id_Tipo);
            return View(vehiculo);
        }

        // GET: Vehiculos/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var vehiculo = await _context.Vehiculos
                .Include(v => v.Cliente)
                .Include(v => v.TipoVehiculo)
                .FirstOrDefaultAsync(v => v.NoPlaca == id);

            if (vehiculo == null) return NotFound();

            return View(vehiculo);
        }

        // POST: Vehiculos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo != null)
            {
                _context.Vehiculos.Remove(vehiculo);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool VehiculoExists(string id)
        {
            return _context.Vehiculos.Any(e => e.NoPlaca == id);
        }

        // Método para cargar SelectLists de clientes y tipos
        private void LoadSelectLists(int? clienteId = null, int? tipoId = null)
        {
            // Solo clientes con Estado = "No registrado"
            var clientes = _context.Clientes
                .Where(c => c.Estado == "No registrado")
                .Select(c => new SelectListItem
                {
                    Value = c.Id_Cliente.ToString(),
                    Text = c.NumeroDocumentacion + " - " + c.Nombres + " " + c.Apellidos
                }).ToList();

            ViewData["Id_Cliente"] = new SelectList(clientes, "Value", "Text", clienteId);
            ViewData["Id_Tipo"] = new SelectList(_context.TipoVehiculos, "Id_Tipo", "Descripcion", tipoId);
        }
    }
}
