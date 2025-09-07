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
    public class VehiculoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VehiculoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Vehiculoes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Vehiculos.Include(v => v.Cliente).Include(v => v.TipoVehiculo);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Vehiculoes/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehiculo = await _context.Vehiculos
                .Include(v => v.Cliente)
                .Include(v => v.TipoVehiculo)
                .FirstOrDefaultAsync(m => m.NoPlaca == id);
            if (vehiculo == null)
            {
                return NotFound();
            }

            return View(vehiculo);
        }

        // GET: Vehiculoes/Create
        public IActionResult Create()
        {
            ViewData["Id_Cliente"] = new SelectList(_context.Clientes, "Id_Cliente", "Apellidos");
            ViewData["Id_Tipo"] = new SelectList(_context.TipoVehiculos, "Id_Tipo", "Descripcion");
            return View();
        }

        // POST: Vehiculoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NoPlaca,Marca,Color,Id_Cliente,Id_Tipo")] Vehiculo vehiculo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vehiculo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Id_Cliente"] = new SelectList(_context.Clientes, "Id_Cliente", "Apellidos", vehiculo.Id_Cliente);
            ViewData["Id_Tipo"] = new SelectList(_context.TipoVehiculos, "Id_Tipo", "Descripcion", vehiculo.Id_Tipo);
            return View(vehiculo);
        }

        // GET: Vehiculoes/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null)
            {
                return NotFound();
            }
            ViewData["Id_Cliente"] = new SelectList(_context.Clientes, "Id_Cliente", "Apellidos", vehiculo.Id_Cliente);
            ViewData["Id_Tipo"] = new SelectList(_context.TipoVehiculos, "Id_Tipo", "Descripcion", vehiculo.Id_Tipo);
            return View(vehiculo);
        }

        // POST: Vehiculoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("NoPlaca,Marca,Color,Id_Cliente,Id_Tipo")] Vehiculo vehiculo)
        {
            if (id != vehiculo.NoPlaca)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vehiculo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehiculoExists(vehiculo.NoPlaca))
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
            ViewData["Id_Cliente"] = new SelectList(_context.Clientes, "Id_Cliente", "Apellidos", vehiculo.Id_Cliente);
            ViewData["Id_Tipo"] = new SelectList(_context.TipoVehiculos, "Id_Tipo", "Descripcion", vehiculo.Id_Tipo);
            return View(vehiculo);
        }

        // GET: Vehiculoes/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehiculo = await _context.Vehiculos
                .Include(v => v.Cliente)
                .Include(v => v.TipoVehiculo)
                .FirstOrDefaultAsync(m => m.NoPlaca == id);
            if (vehiculo == null)
            {
                return NotFound();
            }

            return View(vehiculo);
        }

        // POST: Vehiculoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo != null)
            {
                _context.Vehiculos.Remove(vehiculo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehiculoExists(string id)
        {
            return _context.Vehiculos.Any(e => e.NoPlaca == id);
        }
    }
}
