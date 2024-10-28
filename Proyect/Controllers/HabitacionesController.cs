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
    public class HabitacionesController : Controller
    {
        private readonly ProyectContext _context;

        public HabitacionesController(ProyectContext context)
        {
            _context = context;
        }

        // GET: Habitaciones
        public async Task<IActionResult> Index()
        {
            var proyectContext = _context.Habitaciones.Include(h => h.IdTipoHabitacionNavigation);
            return View(await proyectContext.ToListAsync());
        }

        // GET: Habitaciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var habitacione = await _context.Habitaciones
                .Include(h => h.IdTipoHabitacionNavigation)
                .Include(h => h.HabitacionMuebles)
                .ThenInclude(hm => hm.IdMuebleNavigation)
                .FirstOrDefaultAsync(m => m.IdHabitacion == id);

            if (habitacione == null)
            {
                return NotFound();
            }

            return View(habitacione);
        }

        // GET: Habitaciones/Create
        public IActionResult Create()
        {
            ViewData["IdTipoHabitacion"] = new SelectList(_context.TipoHabitaciones, "IdTipoHabitacion", "Nombre");
            ViewData["Muebles"] = _context.Muebles.Select(m => new { m.IdMueble, m.Nombre }).ToList();
            return View();
        }

        // POST: Habitaciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdHabitacion,Nombre,IdTipoHabitacion,Estado,Descripcion,Precio")] Habitacione habitacione, int[] mueblesSeleccionados)
        {
            if (ModelState.IsValid)
            {
                // Guardar la habitación
                _context.Add(habitacione);
                await _context.SaveChangesAsync();

                // Guardar la relación con los muebles
                if (mueblesSeleccionados != null)
                {
                    foreach (var muebleId in mueblesSeleccionados)
                    {
                        var habitacionMueble = new HabitacionMueble
                        {
                            IdHabitacion = habitacione.IdHabitacion,
                            IdMueble = muebleId,
                            Cantidad = 1 // Definir la cantidad como desees
                        };
                        _context.HabitacionMuebles.Add(habitacionMueble);
                    }

                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["IdTipoHabitacion"] = new SelectList(_context.TipoHabitaciones, "IdTipoHabitacion", "Nombre", habitacione.IdTipoHabitacion);
            ViewData["Muebles"] = new SelectList(_context.Muebles, "IdMueble", "Nombre");
            return View(habitacione);
        }

        // GET: Habitaciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var habitacione = await _context.Habitaciones
                .Include(h => h.HabitacionMuebles)
                .ThenInclude(hm => hm.IdMuebleNavigation)
                .FirstOrDefaultAsync(h => h.IdHabitacion == id);

            if (habitacione == null)
            {
                return NotFound();
            }

            ViewData["IdTipoHabitacion"] = new SelectList(_context.TipoHabitaciones, "IdTipoHabitacion", "Nombre", habitacione.IdTipoHabitacion);

            // Crea una lista de SelectListItem para los muebles
            var muebles = await _context.Muebles.ToListAsync();
            var mueblesSeleccionados = habitacione.HabitacionMuebles.Select(hm => hm.IdMueble).ToArray();

            ViewData["Muebles"] = muebles.Select(m => new SelectListItem
            {
                Value = m.IdMueble.ToString(),
                Text = m.Nombre,
                Selected = mueblesSeleccionados.Contains(m.IdMueble) // Marca el mueble si ya está seleccionado
            }).ToList();

            return View(habitacione);
        }

        // POST: Habitaciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdHabitacion,Nombre,IdTipoHabitacion,Estado,Descripcion,Precio")] Habitacione habitacione, int[] mueblesSeleccionados, int[] cantidadMuebles)
        {
            if (id != habitacione.IdHabitacion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Actualizar la habitación
                    _context.Update(habitacione);
                    await _context.SaveChangesAsync();

                    // Obtener los muebles anteriores
                    var mueblesAnteriores = _context.HabitacionMuebles.Where(hm => hm.IdHabitacion == id).ToList();

                    // Actualizar muebles existentes o agregar nuevos
                    foreach (var muebleId in mueblesSeleccionados)
                    {
                        // Si el mueble ya existe, actualiza su cantidad
                        var muebleExistente = mueblesAnteriores.FirstOrDefault(hm => hm.IdMueble == muebleId);
                        if (muebleExistente != null)
                        {
                            // Actualiza la cantidad si está en la lista de seleccionados
                            var indice = Array.IndexOf(mueblesSeleccionados, muebleId);
                            if (indice >= 0 && cantidadMuebles != null && indice < cantidadMuebles.Length)
                            {
                                muebleExistente.Cantidad = cantidadMuebles[indice];
                            }
                        }
                        else
                        {
                            // Si el mueble no existe, lo agrega
                            var nuevoMueble = new HabitacionMueble
                            {
                                IdHabitacion = habitacione.IdHabitacion,
                                IdMueble = muebleId,
                                Cantidad = cantidadMuebles != null && Array.IndexOf(mueblesSeleccionados, muebleId) < cantidadMuebles.Length
                                    ? cantidadMuebles[Array.IndexOf(mueblesSeleccionados, muebleId)]
                                    : 1 // Cantidad por defecto si no se proporciona
                            };
                            _context.HabitacionMuebles.Add(nuevoMueble);
                        }
                    }

                    // Eliminar muebles que ya no están seleccionados
                    var mueblesAEliminar = mueblesAnteriores.Where(hm => !mueblesSeleccionados.Contains(hm.IdMueble)).ToList();
                    _context.HabitacionMuebles.RemoveRange(mueblesAEliminar);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HabitacioneExists(habitacione.IdHabitacion))
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

            ViewData["IdTipoHabitacion"] = new SelectList(_context.TipoHabitaciones, "IdTipoHabitacion", "Nombre", habitacione.IdTipoHabitacion);
            ViewData["Muebles"] = new SelectList(_context.Muebles, "IdMueble", "Nombre");
            return View(habitacione);
        }


        // GET: Habitaciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var habitacione = await _context.Habitaciones
                .Include(h => h.IdTipoHabitacionNavigation)
                .Include(h => h.HabitacionMuebles)
                .ThenInclude(hm => hm.IdMuebleNavigation)
                .FirstOrDefaultAsync(m => m.IdHabitacion == id);

            if (habitacione == null)
            {
                return NotFound();
            }

            return View(habitacione);
        }

        // POST: Habitaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var habitacione = await _context.Habitaciones.FindAsync(id);
            if (habitacione != null)
            {
                // Eliminar la relación con los muebles primero
                var mueblesRelacionados = _context.HabitacionMuebles.Where(hm => hm.IdHabitacion == id);
                _context.HabitacionMuebles.RemoveRange(mueblesRelacionados);

                _context.Habitaciones.Remove(habitacione);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HabitacioneExists(int id)
        {
            return _context.Habitaciones.Any(e => e.IdHabitacion == id);
        }
    }
}
