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
    public class AbonosController : Controller
    {
        private readonly ProyectContext _context;

        public AbonosController(ProyectContext context)
        {
            _context = context;
        }

        // GET: Abonos/Index
        public async Task<IActionResult> Index(int idReserva)
        {
            // Obtiene los abonos asociados a la reserva
            var abonos = await _context.Abonos
                .Include(a => a.IdReservaNavigation)
                .Include(a => a.IdEstadoAbonoNavigation) // Opcional: incluye otros datos relacionados
                .Where(a => a.IdReserva == idReserva) // Filtra por la reserva específica
                .ToListAsync();

            // Pasar el IdReserva a la vista para mantenerlo visible
            ViewBag.IdReserva = idReserva;

            return View(abonos);
        }





        // GET: Abonos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var abono = await _context.Abonos
                .Include(a => a.IdEstadoAbonoNavigation)
                .Include(a => a.IdReservaNavigation)
                .FirstOrDefaultAsync(m => m.IdAbono == id);
            if (abono == null)
            {
                return NotFound();
            }

            return View(abono);
        }
        // GET: Abonos/Create
        public IActionResult Create(int idReserva)
        {
            // Obtener la reserva asociada al idReserva
            var reserva = _context.Reservas.FirstOrDefault(r => r.IdReserva == idReserva);

            if (reserva == null)
            {
                return NotFound();
            }

            // Calcular el total abonado hasta el momento
            var totalAbonado = _context.Abonos
                .Where(a => a.IdReserva == idReserva)
                .Sum(a => (decimal?)a.ValorAbono) ?? 0; // Manejo de valores nulos

            // Calcular el saldo pendiente
            var pendiente = Math.Max(0, Math.Floor((double)(reserva.Total - totalAbonado)));

            // Crear el modelo Abono con valores iniciales
            var abono = new Abono
            {
                IdReserva = idReserva,
                Valordeuda = Math.Floor(reserva.Total), // Asegurando que el valor sea decimal
                Pendiente = (decimal)pendiente // Convertimos pendiente a decimal
            };

            // Crear el listado de EstadosAbono para el select
            ViewData["IdEstadoAbono"] = new SelectList(_context.EstadosAbonos, "IdEstadoAbono", "Nombre");

            return View(abono);
        }


        // POST: Abonos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("IdAbono,IdReserva,ValorAbono,Porcentaje,Valordeuda,Pendiente,IdEstadoAbono")] Abono abono,
            IFormFile comprobanteFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Asignar la fecha actual al campo FechaAbono
                    abono.FechaAbono = DateTime.Now;

                    // Obtener la reserva asociada
                    var reserva = await _context.Reservas.FirstOrDefaultAsync(r => r.IdReserva == abono.IdReserva);
                    if (reserva == null)
                    {
                        return NotFound();
                    }

                    // Calcular el total abonado hasta el momento
                    var totalAbonado = await _context.Abonos
                        .Where(a => a.IdReserva == abono.IdReserva)
                        .SumAsync(a => (decimal?)a.ValorAbono) ?? 0;

                    // Asignar el valor de la deuda desde la reserva
                    abono.Valordeuda = reserva.Total;

                    // Calcular el saldo pendiente después del abono
                    abono.Pendiente = Math.Max(0, abono.Valordeuda - (totalAbonado + abono.ValorAbono));

                    // Calcular el porcentaje abonado
                    abono.Porcentaje = abono.Valordeuda > 0
                        ? Math.Round((abono.ValorAbono / abono.Valordeuda) * 100, 2)
                        : 0;

                    // Procesar el archivo comprobante si existe
                    if (comprobanteFile != null && comprobanteFile.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await comprobanteFile.CopyToAsync(memoryStream);
                            abono.Comprobante = memoryStream.ToArray(); // Convertir a byte[]
                        }
                    }

                    // Guardar el abono en la base de datos
                    _context.Add(abono);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", new { idReserva = abono.IdReserva });
                }
                catch (Exception ex)
                {
                    // Registrar el error en los logs
                    Console.WriteLine($"Error al crear abono: {ex.Message}");
                    ModelState.AddModelError("", "Se produjo un error al crear el abono. Intenta nuevamente.");
                }
            }

            // Si hay errores en el modelo, cargar los datos para la vista
            ViewData["IdEstadoAbono"] = new SelectList(_context.EstadosAbonos, "IdEstadoAbono", "Nombre", abono.IdEstadoAbono);
            return View(abono);
        }














        // GET: Abonos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var abono = await _context.Abonos.FindAsync(id);
            if (abono == null)
            {
                return NotFound();
            }
            ViewData["IdEstadoAbono"] = new SelectList(_context.EstadosAbonos, "IdEstadoAbono", "IdEstadoAbono", abono.IdEstadoAbono);
            ViewData["IdReserva"] = new SelectList(_context.Reservas, "IdReserva", "IdReserva", abono.IdReserva);
            return View(abono);
        }

        // POST: Abonos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAbono,IdReserva,FechaAbono,Valordeuda,Pendiente,ValorAbono,Porcentaje,Comprobante,IdEstadoAbono")] Abono abono)
        {
            if (id != abono.IdAbono)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(abono);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AbonoExists(abono.IdAbono))
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
            ViewData["IdEstadoAbono"] = new SelectList(_context.EstadosAbonos, "IdEstadoAbono", "IdEstadoAbono", abono.IdEstadoAbono);
            ViewData["IdReserva"] = new SelectList(_context.Reservas, "IdReserva", "IdReserva", abono.IdReserva);
            return View(abono);
        }

        // GET: Abonos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var abono = await _context.Abonos
                .Include(a => a.IdEstadoAbonoNavigation)
                .Include(a => a.IdReservaNavigation)
                .FirstOrDefaultAsync(m => m.IdAbono == id);
            if (abono == null)
            {
                return NotFound();
            }

            return View(abono);
        }

        // POST: Abonos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var abono = await _context.Abonos.FindAsync(id);
            if (abono != null)
            {
                _context.Abonos.Remove(abono);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AbonoExists(int id)
        {
            return _context.Abonos.Any(e => e.IdAbono == id);
        }
    }
}
