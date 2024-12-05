using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyect.Models;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Globalization;


namespace Proyect.Controllers
{
    [Authorize]
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
                .Include(r => r.IdPaqueteNavigation);
            return View(await proyectContext.ToListAsync());
        }

        // GET: Reservas/Details/5
        public IActionResult Details(int id)
        {
            // Obtener la reserva con todos los detalles
            var reserva = _context.Reservas
                .Include(r => r.IdClienteNavigation)  // Incluir cliente
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
public async Task<IActionResult> DescargarPDF(int id)
    {
        // Obtener los detalles de la reserva
        var reserva = await _context.Reservas
            .Include(r => r.IdClienteNavigation)
            .Include(r => r.DetallePaquetes)
            .ThenInclude(dp => dp.IdPaqueteNavigation)
            .Include(r => r.DetalleServicios)
            .ThenInclude(ds => ds.IdServicioNavigation)
            .FirstOrDefaultAsync(r => r.IdReserva == id);

        if (reserva == null)
        {
            return NotFound("Reserva no encontrada.");
        }

        // Crear un nuevo documento PDF
        using (var document = new PdfDocument())
        {
            var page = document.AddPage();
            var graphics = XGraphics.FromPdfPage(page);

                // Configurar estilos de fuente
                var fontTitle = new XFont("Arial Bold", 14); // Negrita
                var fontSubtitle = new XFont("Arial", 14);
                var fontNormal = new XFont("Arial", 12);    // Texto normal

                // Crear cultura colombiana
                var colombianCulture = new CultureInfo("es-CO");

                // Definir colores de la temática (Verde y blanco, inspirados en el Nacional)
                var greenColor = XBrushes.Green; // Verde Nacional
                var whiteColor = XBrushes.White; // Blanco para contraste
                

                // Títulos
                var title = "Detalles de la Reserva";
                var titleWidth = graphics.MeasureString(title, fontTitle).Width;
                graphics.DrawString(title, fontTitle, greenColor, new XPoint((page.Width - titleWidth) / 2, 40)); // Centrado

                // Datos del cliente
                graphics.DrawString("Datos del Cliente", fontSubtitle, greenColor, new XPoint(40, 80));
                graphics.DrawString($"Cliente: {reserva.IdClienteNavigation.Nombre} {reserva.IdClienteNavigation.Apellido}", fontNormal, XBrushes.Black, new XPoint(40, 120));
                graphics.DrawString($"Documento: {reserva.IdClienteNavigation.Documento}", fontNormal, XBrushes.Black, new XPoint(40, 140));
                graphics.DrawString($"Correo: {reserva.IdClienteNavigation.CorreoElectronico}", fontNormal, XBrushes.Black, new XPoint(40, 160));

                // Detalles de la reserva
                graphics.DrawString($"Fecha de Reserva: {reserva.FechaReserva.ToShortDateString()}", fontNormal, XBrushes.Black, new XPoint(40, 200));
                graphics.DrawString($"Fecha de Inicio: {reserva.FechaInicio.ToShortDateString()}", fontNormal, XBrushes.Black, new XPoint(40, 220));
                graphics.DrawString($"Fecha de Fin: {reserva.FechaFin.ToShortDateString()}", fontNormal, XBrushes.Black, new XPoint(40, 240));

                // Detalles de los paquetes
                graphics.DrawString("Paquetes Seleccionados:", fontSubtitle, greenColor, new XPoint(40, 280));
                int yPosition = 300;
                foreach (var detallePaquete in reserva.DetallePaquetes)
                {
                    var precioPaquete = detallePaquete.Precio.ToString("C", colombianCulture);
                    graphics.DrawString($"- {detallePaquete.IdPaqueteNavigation.Nombre} - Precio: {precioPaquete}", fontNormal, XBrushes.Black, new XPoint(40, yPosition));
                    yPosition += 20;
                }

                // Detalles de los servicios
                graphics.DrawString("Servicios Seleccionados:", fontSubtitle, greenColor, new XPoint(40, yPosition + 20));
                yPosition += 40;
                foreach (var detalleServicio in reserva.DetalleServicios)
                {
                    var precioServicio = detalleServicio.Precio.ToString("C", colombianCulture);
                    graphics.DrawString($"- {detalleServicio.IdServicioNavigation.Nombre} - Precio: {precioServicio} - Cantidad: {detalleServicio.Cantidad}", fontNormal, XBrushes.Black, new XPoint(40, yPosition));
                    yPosition += 20;
                }

                // Formatear total a moneda colombiana
                var totalReserva = reserva.Total.ToString("C", colombianCulture);
                graphics.DrawString($"Total: {totalReserva}", fontNormal, XBrushes.Black, new XPoint(40, yPosition));



                // Guardar PDF en memoria
                using (var stream = new MemoryStream())
            {
                document.Save(stream, false);
                return File(stream.ToArray(), "application/pdf", $"Reserva_{id}.pdf");
            }
        }
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

            var clientes = await _context.Clientes
                .Select(c => new { c.IdCliente, ClienteInfo = $"{c.Nombre} {c.Apellido}" })
                .ToListAsync();

            ViewBag.IdCliente = new SelectList(clientes, "IdCliente", "ClienteInfo");

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
        // Acción para crear un cliente desde el modal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCliente([Bind("IdCliente,TipoDocumento,Documento,Nombre,Apellido,Direccion,Celular,CorreoElectronico,Estado")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();
                return Json(new { success = true, id = cliente.IdCliente, name = $"{cliente.Nombre} {cliente.Apellido}" });
            }

            return Json(new { success = false, message = "Error al crear el cliente." });
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Obtener la reserva con relaciones
            var reserva = _context.Reservas
                .Include(r => r.IdMetodoPagoNavigation)
                .Include(r => r.IdEstadoReservaNavigation)
                .Include(r => r.DetalleServicios)
                .Include(r => r.DetallePaquetes)
                .FirstOrDefault(r => r.IdReserva == id);

            if (reserva == null)
            {
                return NotFound();
            }

            // Calcular subtotal, IVA y total
            decimal precioPaquete = reserva.DetallePaquetes.FirstOrDefault()?.Precio ?? 0m;
            decimal subtotal = reserva.DetalleServicios.Sum(ds => ds.Precio * ds.Cantidad) + precioPaquete;
            decimal iva = subtotal * 0.19m; // IVA del 19%
            decimal total = subtotal + iva - reserva.Descuento;

            // Pasar datos al ViewBag
            ViewBag.PrecioPaquete = precioPaquete.ToString("C", new System.Globalization.CultureInfo("es-CO"));
            ViewBag.Paquetes = new SelectList(_context.Paquetes, "IdPaquete", "Nombre", reserva.IdPaquete);
            ViewBag.MetodoPago = new SelectList(_context.MetodoPagos, "IdMetodoPago", "Nombre", reserva.IdMetodoPago);
            ViewBag.EstadoReserva = new SelectList(_context.EstadoReservas, "IdEstadoReserva", "Descripcion", reserva.IdEstadoReserva);

            // Servicios con precios
            ViewBag.Servicios = _context.Servicios
               .Select(s => new
               {
                   IdServicio = s.IdServicio,
                   Nombre = s.Nombre,
                   NombreConPrecio = $"{s.Nombre} - {s.Precio:C2}",
                   Precio = s.Precio // Aseguramos que el precio está disponible
               }).ToList();

            // Servicios seleccionados
            ViewBag.ServiciosSeleccionados = reserva.DetalleServicios.Select(ds => ds.IdServicio).ToList();

            // Pasar el cálculo al modelo para la vista
            reserva.Subtotal = subtotal;
            reserva.Iva = iva;
            reserva.Total = total;

            return View(reserva);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Reserva reserva, List<int> serviciosSeleccionados)
        {
            if (id != reserva.IdReserva)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                // Recargar datos para la vista
                CargarDatosVistaEdit();
                return View(reserva);
            }

            var reservaDb = _context.Reservas
                .Include(r => r.DetalleServicios)
                .Include(r => r.DetallePaquetes)
                .FirstOrDefault(r => r.IdReserva == id);

            if (reservaDb == null)
            {
                return NotFound();
            }

            try
            {
                // Actualizar datos principales
                reservaDb.FechaReserva = reserva.FechaReserva;
                reservaDb.FechaInicio = reserva.FechaInicio;
                reservaDb.FechaFin = reserva.FechaFin;
                reservaDb.IdMetodoPago = reserva.IdMetodoPago;
                reservaDb.IdEstadoReserva = reserva.IdEstadoReserva;
                reservaDb.Descuento = reserva.Descuento;

                // Actualizar detalle de servicios
                ActualizarDetalleServicios(reservaDb, serviciosSeleccionados);

                // Calcular subtotal, IVA y total
                reservaDb.Subtotal = reservaDb.DetalleServicios.Sum(ds => ds.Precio * ds.Cantidad) +
                                     reservaDb.DetallePaquetes.Sum(dp => dp.Precio);
                reservaDb.Iva = reservaDb.Subtotal * 0.19m;
                reservaDb.Total = reservaDb.Subtotal + reservaDb.Iva - reservaDb.Descuento;

                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Ocurrió un error al actualizar la reserva: {ex.Message}");
            }

            CargarDatosVistaEdit();
            return View(reserva);
        }

        private void ActualizarDetalleServicios(Reserva reservaDb, List<int> serviciosSeleccionados)
        {
            // Eliminar servicios existentes
            _context.DetalleServicios.RemoveRange(reservaDb.DetalleServicios);

            if (serviciosSeleccionados != null && serviciosSeleccionados.Any())
            {
                foreach (var servicioId in serviciosSeleccionados)
                {
                    var servicio = _context.Servicios.Find(servicioId);
                    if (servicio != null)
                    {
                        reservaDb.DetalleServicios.Add(new DetalleServicio
                        {
                            IdServicio = servicioId,
                            Precio = servicio.Precio,
                            Cantidad = 1,
                            Estado = true
                        });
                    }
                }
            }
        }

        private void CargarDatosVistaEdit()
        {
            ViewBag.Paquetes = new SelectList(_context.Paquetes, "IdPaquete", "Nombre");
            ViewBag.Servicios = _context.Servicios
                .Select(s => new
                {
                    IdServicio = s.IdServicio,
                    NombreConPrecio = $"{s.Nombre} - {s.Precio.ToString("C", new System.Globalization.CultureInfo("es-CO"))}"
                }).ToList();

            ViewBag.MetodosPago = new SelectList(_context.MetodoPagos, "IdMetodoPago", "Nombre");
            ViewBag.EstadosReserva = new SelectList(_context.EstadoReservas, "IdEstadoReserva", "Descripcion");
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
