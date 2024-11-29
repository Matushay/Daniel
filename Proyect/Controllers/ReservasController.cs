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
    public class ReservasController : Controller
    {
        private readonly ProyectContext _context;

        public ReservasController(ProyectContext context)
        {
            _context = context;
        }

        // GET: Reservas
        public async Task<IActionResult> Index()
        {
            var proyectContext = _context.Reservas
                .Include(r => r.IdClienteNavigation)
                .Include(r => r.IdEstadoReservaNavigation)
                .Include(r => r.IdMetodoPagoNavigation)
                .Include(r => r.IdPaqueteNavigation)
                .Include(r => r.IdUsuarioNavigation);
            return View(await proyectContext.ToListAsync());
        }

        // GET: Reservas/Details/5
        public IActionResult Details(int id)
        {
            // Obtener la reserva con todos los detalles
            var reserva = _context.Reservas
                .Include(r => r.IdClienteNavigation)  // Incluir cliente
                .Include(r => r.IdUsuarioNavigation)  // Incluir usuario
                .Include(r => r.IdEstadoReservaNavigation)  // Incluir estado de reserva
                .Include(r => r.IdMetodoPagoNavigation)  // Incluir método de pago
                .Include(r => r.IdPaqueteNavigation)  // Incluir paquete
                .Include(r => r.DetallePaquetes)  // Incluir detalles de paquetes
                .Include(r => r.DetalleServicios)  // Incluir detalles de servicios
                .ThenInclude(ds => ds.IdServicioNavigation)  // Incluir servicios en detalles
                .FirstOrDefault(r => r.IdReserva == id);

            if (reserva == null)
            {
                return NotFound();
            }

            // Cargar la lista de paquetes y servicios seleccionados
            var detallePaquetes = reserva.DetallePaquetes.Select(dp => new
            {
                Paquete = dp.IdPaqueteNavigation.Nombre,
                Precio = dp.Precio
            }).ToList();

            var detalleServicios = reserva.DetalleServicios.Select(ds => new
            {
                Servicio = ds.IdServicioNavigation.Nombre,
                Precio = ds.Precio,
                Cantidad = ds.Cantidad
            }).ToList();

            // Pasar los datos a la vista
            ViewBag.DetallePaquetes = detallePaquetes;
            ViewBag.DetalleServicios = detalleServicios;

            return View(reserva);
        }


        public async Task<IActionResult> Create()
        {
            CargarDatosVista(); // Cargar clientes, métodos de pago y estados de reserva

            // Cargar los paquetes disponibles con Id, Nombre y Precio
            var paquetes = await _context.Paquetes
                .Select(p => new { p.IdPaquete, p.Nombre, p.Precio })
                .ToListAsync();

            ViewBag.IdPaquete = new SelectList(paquetes, "IdPaquete", "Nombre");
            ViewBag.Paquetes = paquetes; // Pasar paquetes para uso en JavaScript

            // Cargar los servicios disponibles con Id, Nombre y Precio
            var servicios = await _context.Servicios
                .Select(s => new { s.IdServicio, s.Nombre, s.Precio })
                .ToListAsync();

            ViewBag.Servicios = servicios; // Pasar servicios a la vista

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdReserva,FechaReserva,FechaInicio,FechaFin,Subtotal,Iva,Total,NoPersonas,IdCliente,IdUsuario,IdEstadoReserva,IdMetodoPago,Descuento")] Reserva reserva, int IdPaquete, List<int> ServiciosSeleccionados)
        {
            if (ModelState.IsValid)
            {
                // Asignar el IdPaquete a la entidad Reserva
                reserva.IdPaquete = IdPaquete;

                // Obtener el paquete seleccionado y su precio
                var paquete = await _context.Paquetes.FindAsync(IdPaquete);
                if (paquete == null)
                {
                    ModelState.AddModelError("IdPaquete", "El paquete seleccionado no existe.");
                    CargarDatosVista();
                    return View(reserva);
                }

                // Crear el detalle del paquete asociado a la reserva
                var detallePaquete = new DetallePaquete
                {
                    IdPaquete = paquete.IdPaquete,
                    Precio = paquete.Precio,
                    Estado = true
                };

                // Obtener los servicios seleccionados y sumar sus precios
                decimal totalServicios = 0;
                foreach (var servicioId in ServiciosSeleccionados)
                {
                    var servicio = await _context.Servicios.FindAsync(servicioId);
                    if (servicio != null)
                    {
                        reserva.DetalleServicios.Add(new DetalleServicio
                        {
                            IdServicio = servicio.IdServicio,
                            Precio = servicio.Precio,
                            Estado = true,
                            Cantidad = 1 // Ajustar según tus necesidades
                        });
                        totalServicios += servicio.Precio;
                    }
                }

                // Calcular el subtotal con el precio del paquete y los servicios seleccionados
                var subtotalConDescuento = (paquete.Precio + totalServicios) * (1 - reserva.Descuento / 100);
                reserva.Subtotal = decimal.Round(subtotalConDescuento, 2, MidpointRounding.AwayFromZero);
                reserva.Iva = decimal.Round(reserva.Subtotal * 0.19m, 2, MidpointRounding.AwayFromZero);
                reserva.Total = decimal.Round(reserva.Subtotal + reserva.Iva, 2, MidpointRounding.AwayFromZero);

                // Agregar el detalle del paquete a la colección de DetallePaquetes de la reserva
                reserva.DetallePaquetes.Add(detallePaquete);

                // Guardar la reserva junto con los detalles de paquete y servicios seleccionados
                _context.Add(reserva);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Si el modelo no es válido, recargar los datos para la vista
            CargarDatosVista();
            return View(reserva);
        }





        public void CargarDatosVista()
        {
            // Cargar clientes disponibles
            ViewData["IdCliente"] = new SelectList(_context.Clientes, "IdCliente", "Nombre");

            // Cargar métodos de pago disponibles
            ViewData["IdMetodoPago"] = new SelectList(_context.MetodoPagos, "IdMetodoPago", "Nombre");

            // Cargar estados de reserva
            ViewData["IdEstadoReserva"] = new SelectList(_context.EstadoReservas, "IdEstadoReserva", "Estados");

            // Cargar usuarios disponibles (si es necesario)
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "Nombre");
        }
        public async Task<IActionResult> AgregarAbono(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }

            // Crear un nuevo abono y asignar la reserva
            var abono = new Abono { IdReserva = id, FechaAbono = DateTime.Now };
            return View("CreateAbono", abono); // Asegúrate de pasar el modelo de Abono con el IdReserva
        }

        public async Task<IActionResult> BuscarClientePorDocumento(string documento)
        {
            if (string.IsNullOrEmpty(documento))
            {
                return Json(null);  // Si no se pasa documento, devolver null
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Documento == documento);

            if (cliente == null)
            {
                return Json(null);  // Si no se encuentra el cliente, devolver null
            }

            // Devolver los datos del cliente en formato JSON
            var clienteInfo = new
            {
                cliente.IdCliente,
                cliente.Nombre,
                cliente.Apellido,
                cliente.Celular,
                cliente.CorreoElectronico
            };

            return Json(clienteInfo);  // Devolver el objeto cliente como JSON
        }











        // GET: Reservas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reservas == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .FirstOrDefaultAsync(m => m.IdReserva == id);

            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);

            if (reserva != null)
            {
                var detallesPaquetes = _context.DetallePaquetes.Where(dp => dp.IdReserva == id);
                _context.DetallePaquetes.RemoveRange(detallesPaquetes);

                var detallesServicios = _context.DetalleServicios.Where(ds => ds.IdReserva == id);
                _context.DetalleServicios.RemoveRange(detallesServicios);

                _context.Reservas.Remove(reserva);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
