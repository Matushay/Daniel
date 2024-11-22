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
    public class EstadosAbonosController : Controller
    {
        private readonly ProyectContext _context;

        public EstadosAbonosController(ProyectContext context)
        {
            _context = context;
        }

        // GET: EstadosAbonos
        public async Task<IActionResult> Index()
        {
            return View(await _context.EstadosAbonos.ToListAsync());
        }

        // GET: EstadosAbonos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estadosAbono = await _context.EstadosAbonos
                .FirstOrDefaultAsync(m => m.IdEstadoAbono == id);
            if (estadosAbono == null)
            {
                return NotFound();
            }

            return View(estadosAbono);
        }

        // GET: EstadosAbonos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EstadosAbonos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEstadoAbono,Nombre,Descripcion")] EstadosAbono estadosAbono)
        {
            if (ModelState.IsValid)
            {
                _context.Add(estadosAbono);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(estadosAbono);
        }

        // GET: EstadosAbonos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estadosAbono = await _context.EstadosAbonos.FindAsync(id);
            if (estadosAbono == null)
            {
                return NotFound();
            }
            return View(estadosAbono);
        }

        // POST: EstadosAbonos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEstadoAbono,Nombre,Descripcion")] EstadosAbono estadosAbono)
        {
            if (id != estadosAbono.IdEstadoAbono)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(estadosAbono);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EstadosAbonoExists(estadosAbono.IdEstadoAbono))
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
            return View(estadosAbono);
        }

        // GET: EstadosAbonos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estadosAbono = await _context.EstadosAbonos
                .FirstOrDefaultAsync(m => m.IdEstadoAbono == id);
            if (estadosAbono == null)
            {
                return NotFound();
            }

            return View(estadosAbono);
        }

        // POST: EstadosAbonos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var estadosAbono = await _context.EstadosAbonos.FindAsync(id);
            if (estadosAbono != null)
            {
                _context.EstadosAbonos.Remove(estadosAbono);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EstadosAbonoExists(int id)
        {
            return _context.EstadosAbonos.Any(e => e.IdEstadoAbono == id);
        }
    }
}
