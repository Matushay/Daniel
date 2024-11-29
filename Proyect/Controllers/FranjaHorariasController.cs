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
    public class FranjaHorariasController : Controller
    {
        private readonly ProyectContext _context;

        public FranjaHorariasController(ProyectContext context)
        {
            _context = context;
        }

        // GET: FranjaHorarias
        public async Task<IActionResult> Index()
        {
            var proyectContext = _context.FranjasHorarias.Include(f => f.IdServicioNavigation);
            return View(await proyectContext.ToListAsync());
        }

        // GET: FranjaHorarias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var franjaHoraria = await _context.FranjasHorarias
                .Include(f => f.IdServicioNavigation)
                .FirstOrDefaultAsync(m => m.IdFranjaHoraria == id);
            if (franjaHoraria == null)
            {
                return NotFound();
            }

            return View(franjaHoraria);
        }

        // GET: FranjaHorarias/Create
        public IActionResult Create()
        {
            ViewData["IdServicio"] = new SelectList(_context.Servicios, "IdServicio", "Nombre");
            return View();
        }

        // POST: FranjaHorarias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdFranjaHoraria,HoraInicio,HoraFin,IdServicio,Capacidad")] FranjaHoraria franjaHoraria)
        {
            if (ModelState.IsValid)
            {
                _context.Add(franjaHoraria);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdServicio"] = new SelectList(_context.Servicios, "IdServicio", "Nombre", franjaHoraria.IdServicio);
            return View(franjaHoraria);
        }

        // GET: FranjaHorarias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var franjaHoraria = await _context.FranjasHorarias.FindAsync(id);
            if (franjaHoraria == null)
            {
                return NotFound();
            }
            ViewData["IdServicio"] = new SelectList(_context.Servicios, "IdServicio", "Nombre", franjaHoraria.IdServicio);
            return View(franjaHoraria);
        }

        // POST: FranjaHorarias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdFranjaHoraria,HoraInicio,HoraFin,IdServicio,Capacidad")] FranjaHoraria franjaHoraria)
        {
            if (id != franjaHoraria.IdFranjaHoraria)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(franjaHoraria);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FranjaHorariaExists(franjaHoraria.IdFranjaHoraria))
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
            ViewData["IdServicio"] = new SelectList(_context.Servicios, "IdServicio", "Nombre", franjaHoraria.IdServicio);
            return View(franjaHoraria);
        }

        // GET: FranjaHorarias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var franjaHoraria = await _context.FranjasHorarias
                .Include(f => f.IdServicioNavigation)
                .FirstOrDefaultAsync(m => m.IdFranjaHoraria == id);
            if (franjaHoraria == null)
            {
                return NotFound();
            }

            return View(franjaHoraria);
        }

        // POST: FranjaHorarias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var franjaHoraria = await _context.FranjasHorarias.FindAsync(id);
            if (franjaHoraria != null)
            {
                _context.FranjasHorarias.Remove(franjaHoraria);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FranjaHorariaExists(int id)
        {
            return _context.FranjasHorarias.Any(e => e.IdFranjaHoraria == id);
        }
    }
}
