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
    [Authorize(Policy = "AccederTipoMuebles")]
    public class TipoMueblesController : Controller
    {
        private readonly ProyectContext _context;

        public TipoMueblesController(ProyectContext context)
        {
            _context = context;
        }

        // GET: TipoMuebles
        public async Task<IActionResult> Index()
        {
            return View(await _context.TipoMuebles.ToListAsync());
        }

        // GET: TipoMuebles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoMueble = await _context.TipoMuebles
                .FirstOrDefaultAsync(m => m.IdTipoMueble == id);
            if (tipoMueble == null)
            {
                return NotFound();
            }

            return View(tipoMueble);
        }

        // GET: TipoMuebles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoMuebles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTipoMueble,Nombre,Descripcion")] TipoMueble tipoMueble)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoMueble);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "El Tipo de Mueble se ha creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(tipoMueble);
        }

        // GET: TipoMuebles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoMueble = await _context.TipoMuebles.FindAsync(id);
            if (tipoMueble == null)
            {
                return NotFound();
            }
            return View(tipoMueble);
        }

        // POST: TipoMuebles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTipoMueble,Nombre,Descripcion")] TipoMueble tipoMueble)
        {
            if (id != tipoMueble.IdTipoMueble)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoMueble);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoMuebleExists(tipoMueble.IdTipoMueble))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessMessage"] = "El Tipo de Mueble se ha editado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(tipoMueble);
        }

        // GET: TipoMuebles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoMueble = await _context.TipoMuebles
                .FirstOrDefaultAsync(m => m.IdTipoMueble == id);
            if (tipoMueble == null)
            {
                return NotFound();
            }

            return View(tipoMueble);
        }

        // POST: TipoMuebles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoMueble = await _context.TipoMuebles.FindAsync(id);
            if (tipoMueble != null)
            {
                _context.TipoMuebles.Remove(tipoMueble);
            }

            var tipomuebleRelacionado = await _context.Muebles.AnyAsync(ps => ps.IdTipoMueble == id);
            if (tipomuebleRelacionado)
            {
                TempData["ErrorMessage"] = "No se puede eliminar el tipo de mueble porque está asociada a uno o mas muebles.";
                return RedirectToAction(nameof(Delete));
            }

            TempData["SuccessMessage"] = "El tipo de mueble se eliminó correctamente.";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoMuebleExists(int id)
        {
            return _context.TipoMuebles.Any(e => e.IdTipoMueble == id);
        }

        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }
    }
}
