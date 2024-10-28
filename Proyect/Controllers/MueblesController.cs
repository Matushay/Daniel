using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyect.Models;

namespace Proyect.Controllers
{
    public class MueblesController : Controller
    {
        private readonly ProyectContext _context;

        public MueblesController(ProyectContext context)
        {
            _context = context;
        }

        // GET: Muebles
        public async Task<IActionResult> Index()
        {
            var proyectContext = _context.Muebles.Include(m => m.IdTipoMuebleNavigation);
            return View(await proyectContext.ToListAsync());
        }

        // GET: Muebles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mueble = await _context.Muebles
                .Include(m => m.IdTipoMuebleNavigation)
                .FirstOrDefaultAsync(m => m.IdMueble == id);
            if (mueble == null)
            {
                return NotFound();
            }

            return View(mueble);
        }

        // GET: Muebles/Create
        public IActionResult Create()
        {
            ViewData["IdTipoMueble"] = new SelectList(_context.TipoMuebles, "IdTipoMueble", "Nombre");
            return View();
        }

        // POST: Muebles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdMueble,Nombre,IdTipoMueble,Descripcion,Estado,FechaRegistro")] Mueble mueble)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mueble);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdTipoMueble"] = new SelectList(_context.TipoMuebles, "IdTipoMueble", "Nombre", mueble.IdTipoMueble);
            return View(mueble);
        }

        // GET: Muebles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mueble = await _context.Muebles.FindAsync(id);
            if (mueble == null)
            {
                return NotFound();
            }
            ViewData["IdTipoMueble"] = new SelectList(_context.TipoMuebles, "IdTipoMueble", "Nombre", mueble.IdTipoMueble);
            return View(mueble);
        }

        // POST: Muebles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdMueble,Nombre,IdTipoMueble,Descripcion,Estado,FechaRegistro")] Mueble mueble)
        {
            if (id != mueble.IdMueble)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mueble);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MuebleExists(mueble.IdMueble))
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
            ViewData["IdTipoMueble"] = new SelectList(_context.TipoMuebles, "IdTipoMueble", "Nombre", mueble.IdTipoMueble);
            return View(mueble);
        }

        // GET: Muebles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mueble = await _context.Muebles
                .Include(m => m.IdTipoMuebleNavigation)
                .FirstOrDefaultAsync(m => m.IdMueble == id);
            if (mueble == null)
            {
                return NotFound();
            }

            return View(mueble);
        }

        // POST: Muebles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mueble = await _context.Muebles.FindAsync(id);
            if (mueble != null)
            {
                _context.Muebles.Remove(mueble);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MuebleExists(int id)
        {
            return _context.Muebles.Any(e => e.IdMueble == id);
        }
    }
}
