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
    public class EstadoReservasController : Controller
    {
        private readonly ProyectContext _context;

        public EstadoReservasController(ProyectContext context)
        {
            _context = context;
        }

        // GET: EstadoReservas
        public async Task<IActionResult> Index()
        {
            return View(await _context.EstadoReservas.ToListAsync());
        }

        // GET: EstadoReservas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estadoReserva = await _context.EstadoReservas
                .FirstOrDefaultAsync(m => m.IdEstadoReserva == id);
            if (estadoReserva == null)
            {
                return NotFound();
            }

            return View(estadoReserva);
        }

        // GET: EstadoReservas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EstadoReservas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEstadoReserva,Estados,Descripcion")] EstadoReserva estadoReserva)
        {
            if (ModelState.IsValid)
            {
                _context.Add(estadoReserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(estadoReserva);
        }

        // GET: EstadoReservas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estadoReserva = await _context.EstadoReservas.FindAsync(id);
            if (estadoReserva == null)
            {
                return NotFound();
            }
            return View(estadoReserva);
        }

        // POST: EstadoReservas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEstadoReserva,Estados,Descripcion")] EstadoReserva estadoReserva)
        {
            if (id != estadoReserva.IdEstadoReserva)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(estadoReserva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EstadoReservaExists(estadoReserva.IdEstadoReserva))
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
            return View(estadoReserva);
        }

        // GET: EstadoReservas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estadoReserva = await _context.EstadoReservas
                .FirstOrDefaultAsync(m => m.IdEstadoReserva == id);
            if (estadoReserva == null)
            {
                return NotFound();
            }

            return View(estadoReserva);
        }

        // POST: EstadoReservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var estadoReserva = await _context.EstadoReservas.FindAsync(id);
            if (estadoReserva != null)
            {
                _context.EstadoReservas.Remove(estadoReserva);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EstadoReservaExists(int id)
        {
            return _context.EstadoReservas.Any(e => e.IdEstadoReserva == id);
        }
    }
}
