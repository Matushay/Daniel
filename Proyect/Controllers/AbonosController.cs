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
    public class AbonosController : Controller
    {
        private readonly ProyectContext _context;

        public AbonosController(ProyectContext context)
        {
            _context = context;
        }

        // GET: Abonos
        public async Task<IActionResult> Index(int? idReserva)
        {
            IQueryable<Abono> abonos = _context.Abonos.Include(a => a.IdReservaNavigation);

            if (idReserva.HasValue)
            {
                abonos = abonos.Where(a => a.IdReserva == idReserva);
                ViewBag.IdReserva = idReserva; // Pasamos idReserva a la vista
            }

            return View(await abonos.ToListAsync());
        }





        // GET: Abonos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var abono = await _context.Abonos
                .Include(a => a.IdReservaNavigation)
                .FirstOrDefaultAsync(m => m.IdAbono == id);
            if (abono == null)
            {
                return NotFound();
            }

            return View(abono);
        }



        // GET: Abonos/Create
        public async Task<IActionResult> Create(int? idReserva)
        {
            if (idReserva == null)
            {
                return NotFound();
            }

            // Obtener la reserva asociada
            var reserva = await _context.Reservas.FindAsync(idReserva);
            if (reserva == null)
            {
                return NotFound();
            }

            // Calcular el total abonado y el pendiente
            var totalAbonado = _context.Abonos
                                        .Where(a => a.IdReserva == idReserva)
                                        .Sum(a => a.CantidadAbono) ?? 0; // Asegurar que sea 0 si no hay abonos

            var pendiente = reserva.Total - totalAbonado;

            // Pasar datos a ViewBag
            ViewBag.IdReserva = idReserva;
            ViewBag.Total = reserva.Total;
            ViewBag.Pendiente = pendiente;
            ViewBag.Porcentaje = 0; // Por defecto 0 para mostrar en la vista

            return View();
        }

        // POST: Abonos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAbono,IdReserva,FechaAbono,Valordeuda,Porcentaje,Subtotal,Iva,Total,Comprobante,CantidadAbono,Estado")] Abono abono, IFormFile comprobanteFile)
        {
            if (ModelState.IsValid)
            {
                // Obtener la reserva para calcular el porcentaje y pendiente
                var reserva = await _context.Reservas.FindAsync(abono.IdReserva);
                if (reserva != null && reserva.Total > 0)
                {
                    // Calcular porcentaje basado en el total del abono
                    abono.Porcentaje = (abono.Total / reserva.Total) * 100;
                }

                // Si se ha seleccionado un archivo para el comprobante
                if (comprobanteFile != null && comprobanteFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        // Copiar el archivo a un MemoryStream
                        await comprobanteFile.CopyToAsync(memoryStream);

                        // Convertir la imagen a un arreglo de bytes
                        abono.Comprobante = memoryStream.ToArray();
                    }
                }

                // Guardar el abono en la base de datos
                _context.Add(abono);
                await _context.SaveChangesAsync();

                // Calcular nuevo total abonado y pendiente después de guardar el abono
                var totalAbonado = _context.Abonos
                                            .Where(a => a.IdReserva == abono.IdReserva)
                                            .Sum(a => a.CantidadAbono) ?? 0;

                var pendiente = reserva.Total - totalAbonado;

                // Pasar los valores a la vista si se necesita volver a mostrar el formulario
                ViewBag.Pendiente = pendiente;
                ViewBag.TotalAbonado = totalAbonado;

                return RedirectToAction(nameof(Index));
            }

            ViewData["IdReserva"] = new SelectList(_context.Reservas, "IdReserva", "IdReserva", abono.IdReserva);
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
            ViewData["IdReserva"] = new SelectList(_context.Reservas, "IdReserva", "IdReserva", abono.IdReserva);
            return View(abono);
        }

        // POST: Abonos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAbono,IdReserva,FechaAbono,Valordeuda,Porcentaje,Subtotal,Iva,Total,Comprobante,CantidadAbono,Estado")] Abono abono)
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
