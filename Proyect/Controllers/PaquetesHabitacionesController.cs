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
    public class PaquetesHabitacionesController : Controller
    {
        private readonly ProyectContext _context;

        public PaquetesHabitacionesController(ProyectContext context)
        {
            _context = context;
        }

        // GET: PaquetesHabitaciones
        public async Task<IActionResult> Index()
        {
            var proyectContext = _context.PaquetesHabitaciones.Include(p => p.IdHabitacionNavigation).Include(p => p.IdPaqueteNavigation);
            return View(await proyectContext.ToListAsync());
        }

        // GET: PaquetesHabitaciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paquetesHabitacione = await _context.PaquetesHabitaciones
                .Include(p => p.IdHabitacionNavigation)
                .Include(p => p.IdPaqueteNavigation)
                .FirstOrDefaultAsync(m => m.IdPaqueteHabitacion == id);
            if (paquetesHabitacione == null)
            {
                return NotFound();
            }

            return View(paquetesHabitacione);
        }

        // GET: PaquetesHabitaciones/Create
        public IActionResult Create()
        {
            ViewData["IdHabitacion"] = new SelectList(_context.Habitaciones, "IdHabitacion", "Nombre");
            ViewData["IdPaquete"] = new SelectList(_context.Paquetes, "IdPaquete", "Nombre");
            return View();
        }

        // POST: PaquetesHabitaciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPaqueteHabitacion,IdPaquete,IdHabitacion,Precio")] PaquetesHabitacione paquetesHabitacione)
        {
            if (ModelState.IsValid)
            {
                _context.Add(paquetesHabitacione);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdHabitacion"] = new SelectList(_context.Habitaciones, "IdHabitacion", "Nombre", paquetesHabitacione.IdHabitacion);
            ViewData["IdPaquete"] = new SelectList(_context.Paquetes, "IdPaquete", "Nombre", paquetesHabitacione.IdPaquete);
            return View(paquetesHabitacione);
        }

        // GET: PaquetesHabitaciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paquetesHabitacione = await _context.PaquetesHabitaciones.FindAsync(id);
            if (paquetesHabitacione == null)
            {
                return NotFound();
            }
            ViewData["IdHabitacion"] = new SelectList(_context.Habitaciones, "IdHabitacion", "Nombre", paquetesHabitacione.IdHabitacion);
            ViewData["IdPaquete"] = new SelectList(_context.Paquetes, "IdPaquete", "Nombre", paquetesHabitacione.IdPaquete);
            return View(paquetesHabitacione);
        }

        // POST: PaquetesHabitaciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPaqueteHabitacion,IdPaquete,IdHabitacion,Precio")] PaquetesHabitacione paquetesHabitacione)
        {
            if (id != paquetesHabitacione.IdPaqueteHabitacion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paquetesHabitacione);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaquetesHabitacioneExists(paquetesHabitacione.IdPaqueteHabitacion))
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
            ViewData["IdHabitacion"] = new SelectList(_context.Habitaciones, "IdHabitacion", "Nombre", paquetesHabitacione.IdHabitacion);
            ViewData["IdPaquete"] = new SelectList(_context.Paquetes, "IdPaquete", "Nombre", paquetesHabitacione.IdPaquete);
            return View(paquetesHabitacione);
        }

        // GET: PaquetesHabitaciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paquetesHabitacione = await _context.PaquetesHabitaciones
                .Include(p => p.IdHabitacionNavigation)
                .Include(p => p.IdPaqueteNavigation)
                .FirstOrDefaultAsync(m => m.IdPaqueteHabitacion == id);
            if (paquetesHabitacione == null)
            {
                return NotFound();
            }

            return View(paquetesHabitacione);
        }

        // POST: PaquetesHabitaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paquetesHabitacione = await _context.PaquetesHabitaciones.FindAsync(id);
            if (paquetesHabitacione != null)
            {
                _context.PaquetesHabitaciones.Remove(paquetesHabitacione);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaquetesHabitacioneExists(int id)
        {
            return _context.PaquetesHabitaciones.Any(e => e.IdPaqueteHabitacion == id);
        }
    }
}
