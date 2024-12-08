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
    public class DetalleHabitacionesController : Controller
    {
        private readonly ProyectContext _context;

        public DetalleHabitacionesController(ProyectContext context)
        {
            _context = context;
        }

        // GET: DetalleHabitaciones
        public async Task<IActionResult> Index()
        {
            var proyectContext = _context.DetalleHabitaciones.Include(d => d.IdHabitacionNavigation).Include(d => d.IdReservaNavigation);
            return View(await proyectContext.ToListAsync());
        }

        // GET: DetalleHabitaciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detalleHabitacione = await _context.DetalleHabitaciones
                .Include(d => d.IdHabitacionNavigation)
                .Include(d => d.IdReservaNavigation)
                .FirstOrDefaultAsync(m => m.IdDetalleHabitacion == id);
            if (detalleHabitacione == null)
            {
                return NotFound();
            }

            return View(detalleHabitacione);
        }

        // GET: DetalleHabitaciones/Create
        public IActionResult Create()
        {
            ViewData["IdHabitacion"] = new SelectList(_context.Habitaciones, "IdHabitacion", "Nombre");
            ViewData["IdReserva"] = new SelectList(_context.Reservas, "IdReserva", "IdReserva");
            return View();
        }

        // POST: DetalleHabitaciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdDetalleHabitacion,IdReserva,IdHabitacion,Precio,Estado")] DetalleHabitacione detalleHabitacione)
        {
            if (ModelState.IsValid)
            {
                _context.Add(detalleHabitacione);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdHabitacion"] = new SelectList(_context.Habitaciones, "IdHabitacion", "Nombre", detalleHabitacione.IdHabitacion);
            ViewData["IdReserva"] = new SelectList(_context.Reservas, "IdReserva", "IdReserva", detalleHabitacione.IdReserva);
            return View(detalleHabitacione);
        }

        // GET: DetalleHabitaciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detalleHabitacione = await _context.DetalleHabitaciones.FindAsync(id);
            if (detalleHabitacione == null)
            {
                return NotFound();
            }
            ViewData["IdHabitacion"] = new SelectList(_context.Habitaciones, "IdHabitacion", "Nombre", detalleHabitacione.IdHabitacion);
            ViewData["IdReserva"] = new SelectList(_context.Reservas, "IdReserva", "IdReserva", detalleHabitacione.IdReserva);
            return View(detalleHabitacione);
        }

        // POST: DetalleHabitaciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdDetalleHabitacion,IdReserva,IdHabitacion,Precio,Estado")] DetalleHabitacione detalleHabitacione)
        {
            if (id != detalleHabitacione.IdDetalleHabitacion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(detalleHabitacione);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DetalleHabitacioneExists(detalleHabitacione.IdDetalleHabitacion))
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
            ViewData["IdHabitacion"] = new SelectList(_context.Habitaciones, "IdHabitacion", "Nombre", detalleHabitacione.IdHabitacion);
            ViewData["IdReserva"] = new SelectList(_context.Reservas, "IdReserva", "IdReserva", detalleHabitacione.IdReserva);
            return View(detalleHabitacione);
        }

        // GET: DetalleHabitaciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detalleHabitacione = await _context.DetalleHabitaciones
                .Include(d => d.IdHabitacionNavigation)
                .Include(d => d.IdReservaNavigation)
                .FirstOrDefaultAsync(m => m.IdDetalleHabitacion == id);
            if (detalleHabitacione == null)
            {
                return NotFound();
            }

            return View(detalleHabitacione);
        }

        // POST: DetalleHabitaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var detalleHabitacione = await _context.DetalleHabitaciones.FindAsync(id);
            if (detalleHabitacione != null)
            {
                _context.DetalleHabitaciones.Remove(detalleHabitacione);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DetalleHabitacioneExists(int id)
        {
            return _context.DetalleHabitaciones.Any(e => e.IdDetalleHabitacion == id);
        }
    }
}
