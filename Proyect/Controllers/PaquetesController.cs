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
                .Include(p => p.PaquetesServicios)
                    .ThenInclude(ps => ps.IdServicioNavigation) 
                .Include(p => p.PaquetesHabitaciones)
                    .ThenInclude(ph => ph.IdHabitacionNavigation) 
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
            ViewData["Servicios"] = _context.Servicios.Where(s => s.Estado == false).Select(s => new { s.IdServicio, s.Nombre, s.Precio }).ToList();
            ViewData["Habitaciones"] = _context.Habitaciones.Where(s => s.Estado == false).Select(h => new { h.IdHabitacion, h.Nombre, h.Precio }).ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPaquete,Nombre,Descripcion,Precio,Estado")] Paquete paquete, int[] HabitacionesIds, int[] ServiciosIds)
        {
            if (ModelState.IsValid)
            {
                // Validar capacidad de las habitaciones seleccionadas
                if (HabitacionesIds != null)
                {
                    foreach (var habitacionId in HabitacionesIds)
                    {
                        var habitacion = await _context.Habitaciones.FindAsync(habitacionId);
                        if (habitacion != null && habitacion.Cantidad <= 0)
                        {
                            ModelState.AddModelError("", $"La habitación '{habitacion.Nombre}' no esta disponible.");
                            // Recargar datos para la vista
                            ViewData["Servicios"] = _context.Servicios.Where(s => s.Estado == false).Select(s => new { s.IdServicio, s.Nombre, s.Precio }).ToList();
                            ViewData["Habitaciones"] = _context.Habitaciones.Where(h => h.Estado == false).Select(h => new { h.IdHabitacion, h.Nombre, h.Precio }).ToList();
                            return View(paquete);
                        }
                    }
                }

                // Guardar el paquete
                _context.Add(paquete);
                await _context.SaveChangesAsync();

                // Asociar las habitaciones seleccionadas al paquete
                if (HabitacionesIds != null)
                {
                    foreach (var habitacionId in HabitacionesIds)
                    {
                        var habitacion = await _context.Habitaciones.FindAsync(habitacionId);
                        if (habitacion != null)
                        {
                            // Validar capacidad de la habitación
                            if (habitacion.Cantidad > 0)
                            {
                                habitacion.Cantidad--; // Reducir capacidad
                                var paqueteHabitacion = new PaquetesHabitacione
                                {
                                    IdPaquete = paquete.IdPaquete,
                                    IdHabitacion = habitacionId,
                                    Precio = habitacion.Precio
                                };
                                _context.PaquetesHabitaciones.Add(paqueteHabitacion);
                            }
                            else
                            {
                                ModelState.AddModelError("", $"La habitación '{habitacion.Nombre}' no tiene capacidad disponible.");
                                return View(paquete);
                            }
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                // Asociar los servicios seleccionados al paquete
                if (ServiciosIds != null)
                {
                    foreach (var servicioId in ServiciosIds)
                    {
                        var servicio = await _context.Servicios.FindAsync(servicioId);
                        if (servicio != null)
                        {
                            var paqueteServicio = new PaquetesServicio
                            {
                                IdPaquete = paquete.IdPaquete,
                                IdServicio = servicioId,
                                Precio = servicio.Precio
                            };
                            _context.PaquetesServicios.Add(paqueteServicio);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["Servicios"] = _context.Servicios.Where(s => s.Estado == false).Select(s => new { s.IdServicio, s.Nombre, s.Precio }).ToList();
            ViewData["Habitaciones"] = _context.Habitaciones.Where(h => h.Estado == false).Select(h => new { h.IdHabitacion, h.Nombre, h.Precio }).ToList();
            return View(paquete);
        }

        // GET: Paquetes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            // Obtener el paquete y sus relaciones
            var paquete = await _context.Paquetes
                .Include(p => p.PaquetesServicios)
                .Include(p => p.PaquetesHabitaciones)
                .FirstOrDefaultAsync(p => p.IdPaquete == id);

            if (paquete == null)
            {
                return NotFound();
            }

            // Obtener los servicios, incluyendo el estado de selección y precio
            var servicios = _context.Servicios
                .Where(s => s.Estado == false)
                .AsEnumerable()
                .Select(s => new
                {
                    s.IdServicio,
                    s.Nombre,
                    s.Precio,
                    Seleccionado = paquete.PaquetesServicios.Any(ps => ps.IdServicio == s.IdServicio)
                })
                .ToList();

            // Obtener las habitaciones, incluyendo el estado de selección y precio
            var habitaciones = _context.Habitaciones
                .Where(s => s.Estado == false)
                .AsEnumerable()
                .Select(h => new
                {
                    h.IdHabitacion,
                    h.Nombre,
                    h.Precio,
                    Seleccionado = paquete.PaquetesHabitaciones.Any(ph => ph.IdHabitacion == h.IdHabitacion)
                })
                .ToList();

            // Pasar los datos a la vista mediante ViewData
            ViewData["Servicios"] = servicios;
            ViewData["Habitaciones"] = habitaciones;
            ViewData["PrecioTotal"] = paquete.Precio; // Precio inicial del paquete

            return View(paquete);
        }



        // POST: Paquetes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPaquete,Nombre,Descripcion,Precio,Estado")] Paquete paquete, int[] HabitacionesSeleccionadas, int[] ServiciosSeleccionados, decimal PrecioTotal)
        {
            if (id != paquete.IdPaquete)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener habitaciones previamente asociadas
                    var habitacionesExistentes = _context.PaquetesHabitaciones.Where(ph => ph.IdPaquete == id).ToList();

                    // Eliminar relaciones actuales
                    _context.PaquetesHabitaciones.RemoveRange(habitacionesExistentes);

                    // Validar y asociar nuevas habitaciones
                    foreach (var habitacionId in HabitacionesSeleccionadas)
                    {
                        var habitacion = await _context.Habitaciones.FindAsync(habitacionId);
                        if (habitacion != null)
                        {
                            if (habitacion.Cantidad > 0)
                            {
                                habitacion.Cantidad--;
                                _context.PaquetesHabitaciones.Add(new PaquetesHabitacione
                                {
                                    IdPaquete = id,
                                    IdHabitacion = habitacionId
                                });
                            }
                            else
                            {
                                ModelState.AddModelError("", $"La habitación '{habitacion.Nombre}' no tiene capacidad disponible.");
                                return View(paquete);
                            }
                        }
                    }

                    // Actualizar el paquete
                    paquete.Precio = PrecioTotal;
                    _context.Update(paquete);

                    // Manejar los servicios seleccionados
                    var serviciosExistentes = _context.PaquetesServicios.Where(ps => ps.IdPaquete == id).ToList();
                    _context.PaquetesServicios.RemoveRange(serviciosExistentes);

                    foreach (var servicioId in ServiciosSeleccionados)
                    {
                        _context.PaquetesServicios.Add(new PaquetesServicio
                        {
                            IdPaquete = id,
                            IdServicio = servicioId
                        });
                    }

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

            // Recargar datos en caso de error
            ViewData["Servicios"] = _context.Servicios
                .Where(s => s.Estado == false)
                .Select(s => new { s.IdServicio, s.Nombre, s.Precio })
                .ToList();

            ViewData["Habitaciones"] = _context.Habitaciones
                .Where(h => h.Estado == false)
                .Select(h => new { h.IdHabitacion, h.Nombre, h.Precio })
                .ToList();

            ViewData["PrecioTotal"] = PrecioTotal;

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
                .Include(p => p.PaquetesServicios)
                .Include(p => p.PaquetesHabitaciones)
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
            var paquete = await _context.Paquetes
                .Include(p => p.PaquetesServicios)
                .Include(p => p.PaquetesHabitaciones)
                .FirstOrDefaultAsync(p => p.IdPaquete == id);

            if (paquete == null)
            {
                return NotFound();
            }

            // Incrementar la capacidad de las habitaciones asociadas
            foreach (var paqueteHabitacion in paquete.PaquetesHabitaciones)
            {
                var habitacion = await _context.Habitaciones.FindAsync(paqueteHabitacion.IdHabitacion);
                if (habitacion != null)
                {
                    habitacion.Cantidad++; // Aumentar capacidad
                }
            }

            // Eliminar las relaciones de la tabla intermedia PaquetesServicios
            if (paquete.PaquetesServicios.Any())
            {
                _context.PaquetesServicios.RemoveRange(paquete.PaquetesServicios);
            }

            // Eliminar las relaciones de la tabla intermedia PaquetesHabitaciones
            if (paquete.PaquetesHabitaciones.Any())
            {
                _context.PaquetesHabitaciones.RemoveRange(paquete.PaquetesHabitaciones);
            }

            // Eliminar el paquete
            _context.Paquetes.Remove(paquete);

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public IActionResult ActualizarEstado(int id, bool estado)
        {
            var paquete = _context.Paquetes.Find(id);
            if (paquete != null)
            {
                paquete.Estado = estado;
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Habitación no encontrada." });
        }

        private bool PaqueteExists(int id)
        {
            return _context.Paquetes.Any(e => e.IdPaquete == id);
        }
    }
}
