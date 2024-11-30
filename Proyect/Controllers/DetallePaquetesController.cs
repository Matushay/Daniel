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
    public class DetallePaquetesController : Controller
    {
        private readonly ProyectContext _context;

        public DetallePaquetesController(ProyectContext context)
        {
            _context = context;
        }

        // GET: DetallePaquetes
        public async Task<IActionResult> Index()
        {
            var proyectContext = _context.DetallePaquetes.Include(d => d.IdPaqueteNavigation).Include(d => d.IdReservaNavigation);
            return View(await proyectContext.ToListAsync());
        }

        // GET: DetallePaquetes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallePaquete = await _context.DetallePaquetes
                .Include(d => d.IdPaqueteNavigation)
                .Include(d => d.IdReservaNavigation)
                .FirstOrDefaultAsync(m => m.IdDetallePaquete == id);
            if (detallePaquete == null)
            {
                return NotFound();
            }

            return View(detallePaquete);
        }

        // GET: DetallePaquetes/Create
        public IActionResult Create()
        {
            ViewData["IdPaquete"] = new SelectList(_context.Paquetes, "IdPaquete", "Nombre");
            ViewData["IdReserva"] = new SelectList(_context.Reservas, "IdReserva", "IdReserva");
            return View();
        }

        // POST: DetallePaquetes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdDetallePaquete,IdReserva,IdPaquete,Precio,Estado")] DetallePaquete detallePaquete)
        {
            if (ModelState.IsValid)
            {
                _context.Add(detallePaquete);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdPaquete"] = new SelectList(_context.Paquetes, "IdPaquete", "Nombre", detallePaquete.IdPaquete);
            ViewData["IdReserva"] = new SelectList(_context.Reservas, "IdReserva", "IdReserva", detallePaquete.IdReserva);
            return View(detallePaquete);
        }

        // GET: DetallePaquetes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallePaquete = await _context.DetallePaquetes.FindAsync(id);
            if (detallePaquete == null)
            {
                return NotFound();
            }
            ViewData["IdPaquete"] = new SelectList(_context.Paquetes, "IdPaquete", "Nombre", detallePaquete.IdPaquete);
            ViewData["IdReserva"] = new SelectList(_context.Reservas, "IdReserva", "IdReserva", detallePaquete.IdReserva);
            return View(detallePaquete);
        }

        // POST: DetallePaquetes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdDetallePaquete,IdReserva,IdPaquete,Precio,Estado")] DetallePaquete detallePaquete)
        {
            if (id != detallePaquete.IdDetallePaquete)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(detallePaquete);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DetallePaqueteExists(detallePaquete.IdDetallePaquete))
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
            ViewData["IdPaquete"] = new SelectList(_context.Paquetes, "IdPaquete", "Nombre", detallePaquete.IdPaquete);
            ViewData["IdReserva"] = new SelectList(_context.Reservas, "IdReserva", "IdReserva", detallePaquete.IdReserva);
            return View(detallePaquete);
        }

        // GET: DetallePaquetes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallePaquete = await _context.DetallePaquetes
                .Include(d => d.IdPaqueteNavigation)
                .Include(d => d.IdReservaNavigation)
                .FirstOrDefaultAsync(m => m.IdDetallePaquete == id);
            if (detallePaquete == null)
            {
                return NotFound();
            }

            return View(detallePaquete);
        }

        // POST: DetallePaquetes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var detallePaquete = await _context.DetallePaquetes.FindAsync(id);
            if (detallePaquete != null)
            {
                _context.DetallePaquetes.Remove(detallePaquete);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DetallePaqueteExists(int id)
        {
            return _context.DetallePaquetes.Any(e => e.IdDetallePaquete == id);
        }
    }
}
