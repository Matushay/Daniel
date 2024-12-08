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
    [Authorize(Policy = "AccederPaquetes")]

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
            ViewData["Servicios"] = _context.Servicios.Where(s => s.Estado == true).Select(s => new { s.IdServicio, s.Nombre, s.Precio }).ToList();
            ViewData["Habitaciones"] = _context.Habitaciones.Where(s => s.Estado == true).Select(h => new { h.IdHabitacion, h.Nombre, h.Precio }).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPaquete,Nombre,Descripcion,Precio,Estado")] Paquete paquete, int[] HabitacionesIds, int[] ServiciosIds)
        {
            if (ModelState.IsValid)
            {
                // Validar que se haya seleccionado al menos una habitación y un servicio
                if ((HabitacionesIds == null || HabitacionesIds.Length == 0) && (ServiciosIds == null || ServiciosIds.Length == 0))
                {
                    ModelState.AddModelError("", "Debe seleccionar al menos una habitación y un servicio.");
                }
                else if (HabitacionesIds == null || HabitacionesIds.Length == 0)
                {
                    ModelState.AddModelError("", "Debe seleccionar al menos una habitación.");
                }
                else if (ServiciosIds == null || ServiciosIds.Length == 0)
                {
                    ModelState.AddModelError("", "Debe seleccionar al menos un servicio.");
                }
                else
                {
                    // Validar capacidad de las habitaciones seleccionadas
                    foreach (var habitacionId in HabitacionesIds)
                    {
                        var habitacion = await _context.Habitaciones.FindAsync(habitacionId);
                        if (habitacion != null && habitacion.Cantidad <= 0)
                        {
                            ModelState.AddModelError("", $"La habitación '{habitacion.Nombre}' no está disponible.");
                            // Recargar datos para la vista
                            ViewData["Servicios"] = _context.Servicios.Where(s => s.Estado == true).Select(s => new { s.IdServicio, s.Nombre, s.Precio }).ToList();
                            ViewData["Habitaciones"] = _context.Habitaciones.Where(h => h.Estado == true).Select(h => new { h.IdHabitacion, h.Nombre, h.Precio }).ToList();
                            return View(paquete);
                        }
                    }

                    // Guardar el paquete
                    _context.Add(paquete);
                    await _context.SaveChangesAsync();

                    // Asociar las habitaciones seleccionadas al paquete
                    foreach (var habitacionId in HabitacionesIds)
                    {
                        var habitacion = await _context.Habitaciones.FindAsync(habitacionId);
                        if (habitacion != null)
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
                    }
                    await _context.SaveChangesAsync();

                    // Asociar los servicios seleccionados al paquete
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
                    TempData["SuccessMessage"] = "El Paquete se creo correctamente";
                    return RedirectToAction(nameof(Index));
                }
            }

            // Recargar datos en caso de error
            ViewData["Servicios"] = _context.Servicios.Where(s => s.Estado == true).Select(s => new { s.IdServicio, s.Nombre, s.Precio }).ToList();
            ViewData["Habitaciones"] = _context.Habitaciones.Where(h => h.Estado == true).Select(h => new { h.IdHabitacion, h.Nombre, h.Precio }).ToList();
            TempData["SuccessMessage"] = "El Paquete se creo correctamente";
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

            // Cargar los servicios disponibles marcando los seleccionados
            var serviciosSeleccionados = _context.PaquetesServicios
                .Where(ps => ps.IdPaquete == id)
                .Select(ps => ps.IdServicio)
                .ToList();

            ViewData["Servicios"] = _context.Servicios
                .Where(s => s.Estado == true)
                .Select(s => new
                {
                    s.IdServicio,
                    s.Nombre,
                    s.Precio,
                    Seleccionado = serviciosSeleccionados.Contains(s.IdServicio)
                }).ToList();

            // Cargar las habitaciones disponibles marcando las seleccionadas
            var habitacionesSeleccionadas = _context.PaquetesHabitaciones
                .Where(ph => ph.IdPaquete == id)
                .Select(ph => ph.IdHabitacion)
                .ToList();

            ViewData["Habitaciones"] = _context.Habitaciones
                .Where(h => h.Estado == true)
                .Select(h => new
                {
                    h.IdHabitacion,
                    h.Nombre,
                    h.Precio,
                    Seleccionado = habitacionesSeleccionadas.Contains(h.IdHabitacion)
                }).ToList();

            return View(paquete);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPaquete,Nombre,Descripcion,Precio,Estado")] Paquete paquete, int[] HabitacionesIds, int[] ServiciosIds)
        {
            if (id != paquete.IdPaquete)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Validar que se haya seleccionado al menos una habitación y un servicio
                if ((HabitacionesIds == null || HabitacionesIds.Length == 0) && (ServiciosIds == null || ServiciosIds.Length == 0))
                {
                    ModelState.AddModelError("", "Debe seleccionar al menos una habitación y un servicio.");
                }
                else if (HabitacionesIds == null || HabitacionesIds.Length == 0)
                {
                    ModelState.AddModelError("", "Debe seleccionar al menos una habitación.");
                }
                else if (ServiciosIds == null || ServiciosIds.Length == 0)
                {
                    ModelState.AddModelError("", "Debe seleccionar al menos un servicio.");
                }
                else
                {
                    try
                    {
                        // Actualizar el paquete
                        _context.Update(paquete);
                        await _context.SaveChangesAsync();

                        // Actualizar habitaciones asociadas
                        var habitacionesActuales = _context.PaquetesHabitaciones.Where(ph => ph.IdPaquete == id).ToList();
                        _context.PaquetesHabitaciones.RemoveRange(habitacionesActuales);
                        foreach (var habitacionId in HabitacionesIds)
                        {
                            var habitacion = await _context.Habitaciones.FindAsync(habitacionId);
                            if (habitacion != null)
                            {
                                var paqueteHabitacion = new PaquetesHabitacione
                                {
                                    IdPaquete = paquete.IdPaquete,
                                    IdHabitacion = habitacionId,
                                    Precio = habitacion.Precio
                                };
                                _context.PaquetesHabitaciones.Add(paqueteHabitacion);
                            }
                        }

                        // Actualizar servicios asociados
                        var serviciosActuales = _context.PaquetesServicios.Where(ps => ps.IdPaquete == id).ToList();
                        _context.PaquetesServicios.RemoveRange(serviciosActuales);
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
                        TempData["SuccessMessage"] = "El Paquete se editó correctamente.";
                        return RedirectToAction(nameof(Index));
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
                }
            }

            // Recargar datos en caso de error
            ViewData["Servicios"] = _context.Servicios
                .Where(s => s.Estado == true)
                .Select(s => new { s.IdServicio, s.Nombre, s.Precio }).ToList();

            ViewData["Habitaciones"] = _context.Habitaciones
                .Where(h => h.Estado == true)
                .Select(h => new { h.IdHabitacion, h.Nombre, h.Precio }).ToList();

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
            TempData["SuccessMessage"] = "El paquete se elimino correctamente";
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

        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }
    }
}
