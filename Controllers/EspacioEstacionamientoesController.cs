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
    public class EspacioEstacionamientoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EspacioEstacionamientoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EspacioEstacionamientoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.EspacioEstacionamientos.ToListAsync());
        }

        // GET: EspacioEstacionamientoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var espacioEstacionamiento = await _context.EspacioEstacionamientos
                .FirstOrDefaultAsync(m => m.Id_Espacio == id);
            if (espacioEstacionamiento == null)
            {
                return NotFound();
            }

            return View(espacioEstacionamiento);
        }

        // GET: EspacioEstacionamientoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EspacioEstacionamientoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id_Espacio,No_Espacio,Nivel,TipoEspacio,Estado")] EspacioEstacionamiento espacioEstacionamiento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(espacioEstacionamiento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(espacioEstacionamiento);
        }

        // GET: EspacioEstacionamientoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var espacioEstacionamiento = await _context.EspacioEstacionamientos.FindAsync(id);
            if (espacioEstacionamiento == null)
            {
                return NotFound();
            }
            return View(espacioEstacionamiento);
        }

        // POST: EspacioEstacionamientoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id_Espacio,No_Espacio,Nivel,TipoEspacio,Estado")] EspacioEstacionamiento espacioEstacionamiento)
        {
            if (id != espacioEstacionamiento.Id_Espacio)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(espacioEstacionamiento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EspacioEstacionamientoExists(espacioEstacionamiento.Id_Espacio))
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
            return View(espacioEstacionamiento);
        }

        // GET: EspacioEstacionamientoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var espacioEstacionamiento = await _context.EspacioEstacionamientos
                .FirstOrDefaultAsync(m => m.Id_Espacio == id);
            if (espacioEstacionamiento == null)
            {
                return NotFound();
            }

            return View(espacioEstacionamiento);
        }

        // POST: EspacioEstacionamientoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var espacioEstacionamiento = await _context.EspacioEstacionamientos.FindAsync(id);
            if (espacioEstacionamiento != null)
            {
                _context.EspacioEstacionamientos.Remove(espacioEstacionamiento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EspacioEstacionamientoExists(int id)
        {
            return _context.EspacioEstacionamientos.Any(e => e.Id_Espacio == id);
        }
    }
}
