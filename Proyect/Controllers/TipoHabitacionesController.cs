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
    [Authorize(Policy = "AccederTipoHabitaciones")]

    public class TipoHabitacionesController : Controller
    {
        private readonly ProyectContext _context;

        public TipoHabitacionesController(ProyectContext context)
        {
            _context = context;
        }

        // GET: TipoHabitaciones
        public async Task<IActionResult> Index()
        {
            return View(await _context.TipoHabitaciones.ToListAsync());
        }

        // GET: TipoHabitaciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoHabitacione = await _context.TipoHabitaciones
                .FirstOrDefaultAsync(m => m.IdTipoHabitacion == id);
            if (tipoHabitacione == null)
            {
                return NotFound();
            }

            return View(tipoHabitacione);
        }

        // GET: TipoHabitaciones/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoHabitaciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTipoHabitacion,Nombre,Descripcion,Capacidad")] TipoHabitacione tipoHabitacione)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoHabitacione);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "El tipo de habitación se ha creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(tipoHabitacione);
        }

        // GET: TipoHabitaciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoHabitacione = await _context.TipoHabitaciones.FindAsync(id);
            if (tipoHabitacione == null)
            {
                return NotFound();
            }
            return View(tipoHabitacione);
        }

        // POST: TipoHabitaciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTipoHabitacion,Nombre,Descripcion,Capacidad")] TipoHabitacione tipoHabitacione)
        {
            if (id != tipoHabitacione.IdTipoHabitacion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoHabitacione);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoHabitacioneExists(tipoHabitacione.IdTipoHabitacion))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessMessage"] = "El tipo de habitación se ha Editado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(tipoHabitacione);
        }

        // GET: TipoHabitaciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoHabitacione = await _context.TipoHabitaciones
                .FirstOrDefaultAsync(m => m.IdTipoHabitacion == id);
            if (tipoHabitacione == null)
            {
                return NotFound();
            }

            return View(tipoHabitacione);
        }

        // POST: TipoHabitaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoHabitacione = await _context.TipoHabitaciones.FindAsync(id);
            if (tipoHabitacione != null)
            {
                _context.TipoHabitaciones.Remove(tipoHabitacione);
            }

            var tipohabitacionRelacionado = await _context.Habitaciones.AnyAsync(ps => ps.IdTipoHabitacion == id);
            if (tipohabitacionRelacionado)
            {
                TempData["ErrorMessage"] = "No se puede eliminar el tipo de habitacion porque está asociada a una o mas habitaciones.";
                return RedirectToAction(nameof(Delete));
            }

            TempData["SuccessMessage"] = "El tipo de habitación se eliminó correctamente.";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoHabitacioneExists(int id)
        {
            return _context.TipoHabitaciones.Any(e => e.IdTipoHabitacion == id);
        }

        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }
    }
}
