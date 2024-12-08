using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyect.Models;

namespace Proyect.Controllers
{
    [Authorize]
    public class HabitacionMueblesController : Controller
    {
        private readonly ProyectContext _context;

        public HabitacionMueblesController(ProyectContext context)
        {
            _context = context;
        }

        // GET: HabitacionMuebles
        public async Task<IActionResult> Index()
        {
            var proyectContext = _context.HabitacionMuebles.Include(h => h.IdHabitacionNavigation).Include(h => h.IdMuebleNavigation);
            return View(await proyectContext.ToListAsync());
        }

        // GET: HabitacionMuebles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var habitacionMueble = await _context.HabitacionMuebles
                .Include(h => h.IdHabitacionNavigation)
                .Include(h => h.IdMuebleNavigation)
                .FirstOrDefaultAsync(m => m.IdHabitacionMueble == id);
            if (habitacionMueble == null)
            {
                return NotFound();
            }

            return View(habitacionMueble);
        }

        // GET: HabitacionMuebles/Create
        public IActionResult Create()
        {
            ViewData["IdHabitacion"] = new SelectList(_context.Habitaciones, "IdHabitacion", "Nombre");
            ViewData["IdMueble"] = new SelectList(_context.Muebles, "IdMueble", "Nombre");
            return View();
        }

        // POST: HabitacionMuebles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdHabitacionMueble,IdHabitacion,IdMueble,Cantidad")] HabitacionMueble habitacionMueble)
        {
            if (ModelState.IsValid)
            {
                _context.Add(habitacionMueble);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdHabitacion"] = new SelectList(_context.Habitaciones, "IdHabitacion", "Nombre", habitacionMueble.IdHabitacion);
            ViewData["IdMueble"] = new SelectList(_context.Muebles, "IdMueble", "Nombre", habitacionMueble.IdMueble);
            return View(habitacionMueble);
        }

        // GET: HabitacionMuebles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var habitacionMueble = await _context.HabitacionMuebles.FindAsync(id);
            if (habitacionMueble == null)
            {
                return NotFound();
            }
            ViewData["IdHabitacion"] = new SelectList(_context.Habitaciones, "IdHabitacion", "Nombre", habitacionMueble.IdHabitacion);
            ViewData["IdMueble"] = new SelectList(_context.Muebles, "IdMueble", "Nombre", habitacionMueble.IdMueble);
            return View(habitacionMueble);
        }

        // POST: HabitacionMuebles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdHabitacionMueble,IdHabitacion,IdMueble,Cantidad")] HabitacionMueble habitacionMueble)
        {
            if (id != habitacionMueble.IdHabitacionMueble)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(habitacionMueble);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HabitacionMuebleExists(habitacionMueble.IdHabitacionMueble))
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
            ViewData["IdHabitacion"] = new SelectList(_context.Habitaciones, "IdHabitacion", "Nombre", habitacionMueble.IdHabitacion);
            ViewData["IdMueble"] = new SelectList(_context.Muebles, "IdMueble", "Nombre", habitacionMueble.IdMueble);
            return View(habitacionMueble);
        }

        // GET: HabitacionMuebles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var habitacionMueble = await _context.HabitacionMuebles
                .Include(h => h.IdHabitacionNavigation)
                .Include(h => h.IdMuebleNavigation)
                .FirstOrDefaultAsync(m => m.IdHabitacionMueble == id);
            if (habitacionMueble == null)
            {
                return NotFound();
            }

            return View(habitacionMueble);
        }

        // POST: HabitacionMuebles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var habitacionMueble = await _context.HabitacionMuebles.FindAsync(id);
            if (habitacionMueble != null)
            {
                _context.HabitacionMuebles.Remove(habitacionMueble);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HabitacionMuebleExists(int id)
        {
            return _context.HabitacionMuebles.Any(e => e.IdHabitacionMueble == id);
        }
    }
}
