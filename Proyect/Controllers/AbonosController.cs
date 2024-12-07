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
    [Authorize(Policy = "AccederAbonos")]

    public class AbonosController : Controller
    {
        private readonly ProyectContext _context;

        public AbonosController(ProyectContext context)
        {
            _context = context;
        }


        // GET: Abonos/Index
        public async Task<IActionResult> Index(int? idReserva)
        {
            if (idReserva == null)
            {
                return NotFound();
            }

            var abonos = await _context.Abonos
                .Include(a => a.IdReservaNavigation)
                .Where(a => a.IdReserva == idReserva)
                .ToListAsync();

            // Calcular el total abonado (excluyendo los anulados)
            var totalAbonado = abonos
                .Where(a => !a.Anulado)
                .Sum(a => (decimal?)a.ValorAbono) ?? 0;

            var reserva = await _context.Reservas
                .FirstOrDefaultAsync(r => r.IdReserva == idReserva);

            if (reserva == null)
            {
                return NotFound();
            }

            var pendiente = Math.Max(0, reserva.Total - totalAbonado);

            // Verificar si hay algún abono anulado
            var tieneAbonoAnulado = abonos.Any(a => a.Anulado);

            // Pasar los valores necesarios a la vista
            ViewBag.IdReserva = idReserva;
            ViewBag.Pendiente = pendiente;
            ViewBag.TieneAbonoAnulado = tieneAbonoAnulado; // Pasar si hay abono anulado

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

            // Calcular el total abonado hasta el momento (excluyendo los anulados)
            var totalAbonado = _context.Abonos
                .Where(a => a.IdReserva == idReserva && !a.Anulado) // Excluir abonos anulados
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


            return View(abono);
        }

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
                    abono.FechaAbono = DateTime.Now;
                    var reserva = await _context.Reservas.FirstOrDefaultAsync(r => r.IdReserva == abono.IdReserva);
                    if (reserva == null) return NotFound();

                    // Total abonado hasta el momento (excluyendo los anulados)
                    var totalAbonado = await _context.Abonos
                        .Where(a => a.IdReserva == abono.IdReserva && !a.Anulado)
                        .SumAsync(a => (decimal?)a.ValorAbono) ?? 0;

                    // Actualizar valores de deuda y pendiente
                    abono.Valordeuda = reserva.Total;
                    abono.Pendiente = Math.Max(0, abono.Valordeuda - (totalAbonado + abono.ValorAbono));

                    // Calcular el porcentaje de pago y guardarlo en el modelo
                    if (abono.Valordeuda > 0)
                    {
                        abono.Porcentaje = (abono.ValorAbono / abono.Valordeuda) * 100;
                    }
                    else
                    {
                        abono.Porcentaje = 0;
                    }

                    // Guardar el comprobante si es proporcionado
                    if (comprobanteFile != null && comprobanteFile.Length > 0)
                    {
                        using var memoryStream = new MemoryStream();
                        await comprobanteFile.CopyToAsync(memoryStream);
                        abono.Comprobante = memoryStream.ToArray();
                    }

                    // Agregar el abono a la base de datos
                    _context.Add(abono);

                    // Verificar el estado de la reserva
                    var totalAbonoActualizado = totalAbonado + abono.ValorAbono;

                    if (totalAbonoActualizado >= reserva.Total)
                    {
                        // Cambiar a "Confirmado" si el total abonado cubre la reserva
                        reserva.IdEstadoReserva = 2; // Suponiendo que 2 es "Confirmado"
                    }
                    else if (totalAbonoActualizado > 0)
                    {
                        // Cambiar a "Por Confirmar" si hay algún abono pero no está completo
                        reserva.IdEstadoReserva = 4; // Suponiendo que 4 es "Por Confirmar"
                    }

                    // Guardar cambios en la reserva
                    _context.Update(reserva);

                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", new { idReserva = abono.IdReserva });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al crear abono: {ex.Message}");
                    ModelState.AddModelError("", "Se produjo un error al crear el abono. Intenta nuevamente.");
                }
            }
            return View(abono);
        }






        // POST: Abonos/Anular/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Anular(int id)
        {
            var abono = await _context.Abonos.FindAsync(id);
            if (abono == null)
            {
                return NotFound();
            }

            // Marcar el abono como anulado
            abono.Anulado = true;

            try
            {
                _context.Update(abono);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al anular abono: {ex.Message}");
                ModelState.AddModelError("", "Se produjo un error al anular el abono.");
            }

            return RedirectToAction("Index", new { idReserva = abono.IdReserva });
        }
        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }
    }
}
