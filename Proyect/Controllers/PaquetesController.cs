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
            ViewData["Servicios"] = _context.Servicios.Select(s => new { s.IdServicio, s.Nombre, s.Precio }).ToList();
            ViewData["Habitaciones"] = _context.Habitaciones.Select(h => new { h.IdHabitacion, h.Nombre, h.Precio }).ToList();
            return View();
        }

        // POST: Paquetes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPaquete,Nombre,Descripcion,Precio,Estado")] Paquete paquete, int[] HabitacionesIds, int[] ServiciosIds, int[] MueblesIds)
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
                    await _context.SaveChangesAsync();
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
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["Servicios"] = new SelectList(_context.Servicios, "IdServicio", "Nombre");
            ViewData["Habitaciones"] = new SelectList(_context.Habitaciones, "IdHabitacion", "Nombre");
            return View(paquete);
          
        }

        // GET: Paquetes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paquete = await _context.Paquetes
                .Include(p => p.PaquetesServicios)
                .ThenInclude(ps => ps.IdServicioNavigation)
                .FirstOrDefaultAsync(p => p.IdPaquete == id);

            if (paquete == null)
            {
                return NotFound();
            }
            // Crea una lista de SelectList para los Servicios

            ViewData["IdServicio"] = new SelectList(_context.Servicios, "IdServicio", "Nombre", paquete.IdPaquete);

            var servicios = await _context.Servicios
                .Select(s => new
                {
                    s.IdServicio,
                    s.Nombre,
                    s.Precio
                }).ToListAsync();

            var habitaciones = await _context.Habitaciones.Select(h => new
            {
                h.IdHabitacion,
                h.Nombre,
                h.Precio
            }).ToListAsync();

            var serviciosSeleccionados = paquete.PaquetesServicios.Select(ps => ps.IdServicio).ToArray();

            ViewData["Servicios"] = servicios.Select(s => new Servicio
            {
                IdServicio = s.IdServicio,
                Nombre = s.Nombre,
                Precio = s.Precio
            }).ToList();


            var habitacionesSeleccionadas = paquete.PaquetesHabitaciones.Select(ph => ph.IdHabitacion).ToArray(); // lógica para obtener habitaciones seleccionadas
            ViewData["Habitaciones"] = habitaciones.Select(h => new Habitacione
            {
                IdHabitacion = h.IdHabitacion,
                Nombre = h.Nombre,
                Precio = h.Precio
            }).ToList();

            ViewBag.ServiciosSeleccionados = serviciosSeleccionados;
            return View(paquete);
        }
    

        // POST: Paquetes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPaquete,Nombre,Descripcion,Precio,Estado")] Paquete paquete, int?[] ServiciosSeleccionados, int?[] HabitacionesSeleccionadas)
        {
            if (id != paquete.IdPaquete)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Actualizar el paquete
                    _context.Update(paquete);
                    await _context.SaveChangesAsync();

                    // Obtener los servicios anteriores
                    var serviciosAnteriores = _context.PaquetesServicios.Where(ps => ps.IdPaquete == id).ToList();

                    // Actualizar servicios existentes o agregar nuevos
                    foreach (var servicioId in ServiciosSeleccionados)
                    {
                        // Si el servicio ya existe, se actualiza
                        var servicioExistente = serviciosAnteriores.FirstOrDefault(ps => ps.IdServicio == servicioId);
                        if (servicioExistente == null)
                        {
                            // Si el servicio no existe, se agrega
                            var nuevoServicio = new PaquetesServicio
                            {
                                IdPaquete = paquete.IdPaquete,
                                IdServicio = servicioId
                            };
                            _context.PaquetesServicios.Add(nuevoServicio);
                        }
                        // Si ya existe, no es necesario hacer nada
                    }

                    // Eliminar servicios que ya no están seleccionados
                    var serviciosAEliminar = serviciosAnteriores.Where(ps => !ServiciosSeleccionados.Contains(ps.IdServicio)).ToList();
                    _context.PaquetesServicios.RemoveRange(serviciosAEliminar);

                    // Obtener las habitaciones anteriores
                    var habitacionesAnteriores = _context.PaquetesHabitaciones.Where(ph => ph.IdPaquete == id).ToList();

                    // Actualizar habitaciones existentes o agregar nuevas
                    foreach (var habitacionId in HabitacionesSeleccionadas)
                    {
                        // Si la habitación ya existe, se actualiza
                        var habitacionExistente = habitacionesAnteriores.FirstOrDefault(ph => ph.IdHabitacion == habitacionId);
                        if (habitacionExistente == null)
                        {
                            // Si la habitación no existe, se agrega
                            var nuevaHabitacion = new PaquetesHabitacione
                            {
                                IdPaquete = paquete.IdPaquete,
                                IdHabitacion = habitacionId
                            };
                            _context.PaquetesHabitaciones.Add(nuevaHabitacion);
                        }
                        // Si ya existe, no es necesario hacer nada
                    }

                    // Eliminar habitaciones que ya no están seleccionadas
                    var habitacionesAEliminar = habitacionesAnteriores.Where(ph => !HabitacionesSeleccionadas.Contains(ph.IdHabitacion)).ToList();
                    _context.PaquetesHabitaciones.RemoveRange(habitacionesAEliminar);

                    // Guardar los cambios
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
