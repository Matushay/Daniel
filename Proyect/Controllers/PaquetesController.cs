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
    public class PaquetesController : Controller
    {
        private readonly ProyectContext _context;

        public PaquetesController(ProyectContext context)
        {
            _context = context;
        }

        // GET: Paquetes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Paquetes.ToListAsync());
        }

        // GET: Paquetes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paquete = await _context.Paquetes
                .FirstOrDefaultAsync(m => m.IdPaquete == id);
            if (paquete == null)
            {
                return NotFound();
            }

            return View(paquete);
        }

        // GET: Paquetes/Create
        public IActionResult Create()
        {
            // Cargar las habitaciones disponibles
            ViewBag.Habitaciones = new SelectList(_context.Habitaciones.Where(h => h.Estado == true), "IdHabitacion", "Nombre");

            // Cargar los servicios disponibles
            ViewBag.Servicios = new SelectList(_context.Servicios.Where(s => s.Estado == true), "IdServicio", "Nombre");

            return View();
        }


        // POST: Paquetes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPaquete,Nombre,Descripcion,Precio,Estado")] Paquete paquete, int[] HabitacionesIds, int[] ServiciosIds)
        {
            if (ModelState.IsValid)
            {
                // Guardar el paquete
                _context.Add(paquete);
                await _context.SaveChangesAsync();

                // Asociar las habitaciones seleccionadas al paquete
                if (HabitacionesIds != null)
                {
                    foreach (var habitacionId in HabitacionesIds)
                    {
                        var paqueteHabitacion = new PaquetesHabitacione
                        {
                            IdPaquete = paquete.IdPaquete,
                            IdHabitacion = habitacionId
                        };
                        _context.PaquetesHabitaciones.Add(paqueteHabitacion);
                    }
                }

                // Asociar los servicios seleccionados al paquete
                if (ServiciosIds != null)
                {
                    foreach (var servicioId in ServiciosIds)
                    {
                        var paqueteServicio = new PaquetesServicio
                        {
                            IdPaquete = paquete.IdPaquete,
                            IdServicio = servicioId
                        };
                        _context.PaquetesServicios.Add(paqueteServicio);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Recargar las listas de habitaciones y servicios si algo falla
            ViewBag.Habitaciones = new SelectList(_context.Habitaciones.Where(h => h.Estado == true), "IdHabitacion", "Nombre");
            ViewBag.Servicios = new SelectList(_context.Servicios.Where(s => s.Estado == true), "IdServicio", "Nombre");

            return View(paquete);
        }


        // GET: Paquetes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paquete = await _context.Paquetes.FindAsync(id);
            if (paquete == null)
            {
                return NotFound();
            }
            return View(paquete);
        }

        // POST: Paquetes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPaquete,Nombre,Descripcion,Precio,Estado")] Paquete paquete)
        {
            if (id != paquete.IdPaquete)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paquete);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaqueteExists(paquete.IdPaquete))
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
            return View(paquete);
        }

        // GET: Paquetes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paquete = await _context.Paquetes
                .FirstOrDefaultAsync(m => m.IdPaquete == id);
            if (paquete == null)
            {
                return NotFound();
            }

            return View(paquete);
        }

        // POST: Paquetes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paquete = await _context.Paquetes.FindAsync(id);
            if (paquete != null)
            {
                _context.Paquetes.Remove(paquete);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaqueteExists(int id)
        {
            return _context.Paquetes.Any(e => e.IdPaquete == id);
        }
    }
}
