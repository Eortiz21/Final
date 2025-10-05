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
    public class TarifasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TarifasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tarifas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tarifas.ToListAsync());
        }

        // GET: Tarifas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarifa = await _context.Tarifas
                .FirstOrDefaultAsync(m => m.Id_Tarifa == id);
            if (tarifa == null)
            {
                return NotFound();
            }

            return View(tarifa);
        }

        // GET: Tarifas/Create
        public IActionResult Create()
        {
            CargarListaTarifas();   // <- Llenamos el ViewBag con la lista para el dropdown
            return View();
        }

        // POST: Tarifas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id_Tarifa,TipoTarifa,Monto,Descripcion")] Tarifa tarifa)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tarifa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            CargarListaTarifas();   // <- Si hay error, volver a llenar el dropdown
            return View(tarifa);
        }

        // GET: Tarifas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarifa = await _context.Tarifas.FindAsync(id);
            if (tarifa == null)
            {
                return NotFound();
            }

            CargarListaTarifas(tarifa.Id_Tarifa);   // <- Llenamos el ViewBag con selección actual
            return View(tarifa);
        }

        // POST: Tarifas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id_Tarifa,TipoTarifa,Monto,Descripcion")] Tarifa tarifa)
        {
            if (id != tarifa.Id_Tarifa)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tarifa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TarifaExists(tarifa.Id_Tarifa))
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
            CargarListaTarifas(tarifa.Id_Tarifa);  // <- Si hay error, volver a llenar el dropdown
            return View(tarifa);
        }

        // GET: Tarifas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarifa = await _context.Tarifas
                .FirstOrDefaultAsync(m => m.Id_Tarifa == id);
            if (tarifa == null)
            {
                return NotFound();
            }

            return View(tarifa);
        }

        // POST: Tarifas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tarifa = await _context.Tarifas.FindAsync(id);
            if (tarifa != null)
            {
                _context.Tarifas.Remove(tarifa);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TarifaExists(int id)
        {
            return _context.Tarifas.Any(e => e.Id_Tarifa == id);
        }

        /// <summary>
        /// Método para llenar el ViewBag con la lista de tarifas (ID como valor, TipoTarifa como texto).
        /// </summary>
        /// <param name="selectedId">El Id seleccionado (opcional).</param>
        private void CargarListaTarifas(int? selectedId = null)
        {
            var tarifas = _context.Tarifas.ToList();
            ViewBag.Id_Tarifa = new SelectList(tarifas, "Id_Tarifa", "TipoTarifa", selectedId);
        }
    }
}
