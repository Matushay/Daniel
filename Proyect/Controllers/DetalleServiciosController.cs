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
    public class DetalleServiciosController : Controller
    {
        private readonly ProyectContext _context;

        public DetalleServiciosController(ProyectContext context)
        {
            _context = context;
        }

        // GET: DetalleServicios
        public async Task<IActionResult> Index()
        {
            var proyectContext = _context.DetalleServicios.Include(d => d.IdReservaNavigation).Include(d => d.IdServicioNavigation);
            return View(await proyectContext.ToListAsync());
        }

        // GET: DetalleServicios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detalleServicio = await _context.DetalleServicios
                .Include(d => d.IdReservaNavigation)
                .Include(d => d.IdServicioNavigation)
                .FirstOrDefaultAsync(m => m.IdDetalleServicio == id);
            if (detalleServicio == null)
            {
                return NotFound();
            }

            return View(detalleServicio);
        }

        // GET: DetalleServicios/Create
        public IActionResult Create()
        {
            ViewData["IdReserva"] = new SelectList(_context.Reservas, "IdReserva", "IdReserva");
            ViewData["IdServicio"] = new SelectList(_context.Servicios, "IdServicio", "Nombre");
            return View();
        }

        // POST: DetalleServicios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdDetalleServicio,IdReserva,IdServicio,Cantidad,Precio,Estado")] DetalleServicio detalleServicio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(detalleServicio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdReserva"] = new SelectList(_context.Reservas, "IdReserva", "IdReserva", detalleServicio.IdReserva);
            ViewData["IdServicio"] = new SelectList(_context.Servicios, "IdServicio", "Nombre", detalleServicio.IdServicio);
            return View(detalleServicio);
        }

        // GET: DetalleServicios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detalleServicio = await _context.DetalleServicios.FindAsync(id);
            if (detalleServicio == null)
            {
                return NotFound();
            }
            ViewData["IdReserva"] = new SelectList(_context.Reservas, "IdReserva", "IdReserva", detalleServicio.IdReserva);
            ViewData["IdServicio"] = new SelectList(_context.Servicios, "IdServicio", "Nombre", detalleServicio.IdServicio);
            return View(detalleServicio);
        }

        // POST: DetalleServicios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdDetalleServicio,IdReserva,IdServicio,Cantidad,Precio,Estado")] DetalleServicio detalleServicio)
        {
            if (id != detalleServicio.IdDetalleServicio)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(detalleServicio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DetalleServicioExists(detalleServicio.IdDetalleServicio))
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
            ViewData["IdReserva"] = new SelectList(_context.Reservas, "IdReserva", "IdReserva", detalleServicio.IdReserva);
            ViewData["IdServicio"] = new SelectList(_context.Servicios, "IdServicio", "Nombre", detalleServicio.IdServicio);
            return View(detalleServicio);
        }

        // GET: DetalleServicios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detalleServicio = await _context.DetalleServicios
                .Include(d => d.IdReservaNavigation)
                .Include(d => d.IdServicioNavigation)
                .FirstOrDefaultAsync(m => m.IdDetalleServicio == id);
            if (detalleServicio == null)
            {
                return NotFound();
            }

            return View(detalleServicio);
        }

        // POST: DetalleServicios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var detalleServicio = await _context.DetalleServicios.FindAsync(id);
            if (detalleServicio != null)
            {
                _context.DetalleServicios.Remove(detalleServicio);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DetalleServicioExists(int id)
        {
            return _context.DetalleServicios.Any(e => e.IdDetalleServicio == id);
        }
    }
}
