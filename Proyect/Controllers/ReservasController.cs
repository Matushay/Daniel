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
    [Authorize(Policy = "AccederReservas")]
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Anular(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }

            reserva.IdEstadoReserva = 3; // Asumiendo que el ID 3 corresponde a "Anulada"

            try
            {
                _context.Update(reserva);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al anular la reserva: {ex.Message}");
                ModelState.AddModelError("", "Se produjo un error al anular la reserva.");
            }

            return RedirectToAction(nameof(Index));
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

            // Cargar los paquetes disponibles con Id, Nombre y Precio solo si están activos
            var paquetes = await _context.Paquetes
                .Where(p => p.Estado) // Filtrar paquetes activos
                .Select(p => new { p.IdPaquete, p.Nombre, p.Precio })
                .ToListAsync();

            ViewBag.IdPaquete = new SelectList(paquetes, "IdPaquete", "Nombre");
            ViewBag.Paquetes = paquetes; // Pasar paquetes para uso en JavaScript

            // Cargar los servicios disponibles con Id, Nombre y Precio solo si están activos
            var servicios = await _context.Servicios
                .Where(s => s.Estado) // Filtrar servicios activos
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
        public async Task<IActionResult> Create([Bind("IdReserva,FechaReserva,FechaInicio,FechaFin,Subtotal,Iva,Total,NoPersonas,IdCliente,IdUsuario,IdMetodoPago,Descuento")] Reserva reserva, int IdPaquete, List<int> ServiciosSeleccionados)
        {
            if (ModelState.IsValid)
            {
                // Asignar el estado "Pendiente" por defecto
                var estadoPendiente = await _context.EstadoReservas
                    .FirstOrDefaultAsync(e => e.Estados == "Pendiente");

                if (estadoPendiente != null)
                {
                    reserva.IdEstadoReserva = estadoPendiente.IdEstadoReserva;
                }
                else
                {
                    // Si no se encuentra el estado "Pendiente", agregar error
                    ModelState.AddModelError("IdEstadoReserva", "El estado 'Pendiente' no se encuentra en la base de datos.");
                    CargarDatosVista();
                    return View(reserva);
                }

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
                .Include(r => r.IdPaqueteNavigation)  // Aseguramos de incluir el paquete
                .FirstOrDefault(r => r.IdReserva == id);

            if (reserva == null)
            {
                return NotFound();
            }

            // Calcular subtotal, IVA y total
            decimal subtotal = reserva.DetalleServicios.Sum(ds => ds.Precio * ds.Cantidad);

            // Si existe paquete, sumarlo al subtotal
            if (reserva.IdPaqueteNavigation != null)
            {
                subtotal += reserva.IdPaqueteNavigation.Precio;
            }

            decimal iva = subtotal * 0.19m; // IVA del 19%
            decimal total = subtotal + iva - reserva.Descuento; // Restar descuento del total

            // Pasar datos al ViewBag
            ViewBag.Paquetes = new SelectList(_context.Paquetes.Where(p => p.Estado), "IdPaquete", "Nombre", reserva.IdPaquete);
            ViewBag.MetodoPago = new SelectList(_context.MetodoPagos, "IdMetodoPago", "Nombre", reserva.IdMetodoPago);
            ViewBag.EstadoReserva = new SelectList(_context.EstadoReservas, "IdEstadoReserva", "Estados", reserva.IdEstadoReserva);

            // Servicios con precios
            ViewBag.Servicios = _context.Servicios
                .Where(s => s.Estado)
               .Select(s => new
               {
                   IdServicio = s.IdServicio,
                   Nombre = s.Nombre,
                   NombreConPrecio = $"{s.Nombre} - {s.Precio:C2}",
                   Precio = s.Precio // Aseguramos que el precio está disponible
               }).ToList();

            // Servicios seleccionados
            ViewBag.ServiciosSeleccionados = reserva.DetalleServicios.Select(ds => ds.IdServicio).ToList();

            // Asignar el precio del paquete si existe
            if (reserva.IdPaqueteNavigation != null)
            {
                ViewBag.PrecioPaquete = reserva.IdPaqueteNavigation.Precio;
            }
            else
            {
                ViewBag.PrecioPaquete = 0; // O un valor predeterminado en caso de que no haya paquete
            }

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
                CargarDatosVistaEdit();
                return View(reserva);
            }

            // Buscar la reserva existente en la base de datos
            var reservaDb = _context.Reservas
                .Include(r => r.DetalleServicios) // Incluye los servicios asociados
                .Include(r => r.IdPaqueteNavigation) // Incluye la relación con el paquete
                .FirstOrDefault(r => r.IdReserva == id);

            if (reservaDb == null)
            {
                return NotFound();
            }

            try
            {
                // Actualizar los campos de la reserva
                reservaDb.FechaReserva = reserva.FechaReserva;
                reservaDb.FechaInicio = reserva.FechaInicio;
                reservaDb.FechaFin = reserva.FechaFin;
                reservaDb.IdMetodoPago = reserva.IdMetodoPago;
                reservaDb.IdEstadoReserva = reserva.IdEstadoReserva;
                reservaDb.Descuento = reserva.Descuento;

                // Actualizar los servicios seleccionados
                ActualizarDetalleServicios(reservaDb, serviciosSeleccionados);

                // Actualizar el paquete si se seleccionó uno
                ActualizarDetallePaquete(reservaDb, reserva.IdPaquete);

                // Calcular subtotal únicamente con los servicios seleccionados
                decimal subtotal = 0;
                if (serviciosSeleccionados != null && serviciosSeleccionados.Any())
                {
                    foreach (var servicioId in serviciosSeleccionados)
                    {
                        var servicio = _context.Servicios.FirstOrDefault(s => s.IdServicio == servicioId);
                        if (servicio != null)
                        {
                            subtotal += servicio.Precio; // Suma el precio de los servicios seleccionados
                        }
                    }
                }

                // Si se seleccionó un paquete, agregar su precio al subtotal
                if (reservaDb.IdPaqueteNavigation != null)
                {
                    subtotal += reservaDb.IdPaqueteNavigation.Precio;
                }

                // Aplicar descuento y calcular IVA y total
                decimal descuento = (subtotal * reserva.Descuento) / 100;
                decimal subtotalConDescuento = subtotal - descuento;
                decimal iva = subtotalConDescuento * 0.19m; // IVA del 19%
                decimal total = subtotalConDescuento + iva;

                // Guardar los cálculos en la base de datos
                reservaDb.Subtotal = subtotal;
                reservaDb.Iva = iva;
                reservaDb.Total = total;
                // Guardar cambios en la base de datos
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Ocurrió un error al actualizar la reserva: {ex.Message}");
            }

            // Recargar datos para la vista en caso de error
            CargarDatosVistaEdit();
            return View(reserva);
        }

        private void ActualizarDetallePaquete(Reserva reservaDb, int? idPaqueteSeleccionado)
        {
            // Si se seleccionó un paquete diferente o se deseleccionó un paquete (idPaqueteSeleccionado == null)
            if (idPaqueteSeleccionado.HasValue)
            {
                // Verificar si el paquete ya está asociado con la reserva
                var detallePaqueteExistente = _context.DetallePaquetes
                    .FirstOrDefault(dp => dp.IdReserva == reservaDb.IdReserva);

                if (detallePaqueteExistente != null)
                {
                    // Si ya existe un detalle de paquete, lo eliminamos
                    _context.DetallePaquetes.Remove(detallePaqueteExistente);
                }

                // Obtener el precio del nuevo paquete
                var precioPaquete = _context.Paquetes
                    .Where(p => p.IdPaquete == idPaqueteSeleccionado.Value)
                    .Select(p => p.Precio)
                    .FirstOrDefault();

                // Agregar el nuevo paquete
                var nuevoDetallePaquete = new DetallePaquete
                {
                    IdReserva = reservaDb.IdReserva,
                    IdPaquete = idPaqueteSeleccionado.Value,
                    Precio = precioPaquete,
                    Estado = true // Establecer estado como "activo"
                };

                _context.DetallePaquetes.Add(nuevoDetallePaquete);

                // Actualizar el campo IdPaquete en la reserva
                reservaDb.IdPaquete = idPaqueteSeleccionado.Value;

                // Actualizar la relación de navegación
                reservaDb.IdPaqueteNavigation = _context.Paquetes
                    .FirstOrDefault(p => p.IdPaquete == idPaqueteSeleccionado.Value);
            }
            else
            {
                // Si no se seleccionó un paquete (o se deseleccionó), eliminar cualquier detalle de paquete existente
                var detallePaqueteExistente = _context.DetallePaquetes
                    .FirstOrDefault(dp => dp.IdReserva == reservaDb.IdReserva);

                if (detallePaqueteExistente != null)
                {
                    _context.DetallePaquetes.Remove(detallePaqueteExistente);
                }

                // Limpiar el campo IdPaquete en la reserva
                reservaDb.IdPaqueteNavigation = null;
            }
        }

        private void ActualizarDetalleServicios(Reserva reservaDb, List<int> serviciosSeleccionados)
        {
            // Eliminar los servicios existentes que no están en la nueva selección
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
        [HttpGet]

        public IActionResult ObtenerPrecioPaquete(int id)
        {
            // Obtener el paquete por el ID
            var paquete = _context.Paquetes.FirstOrDefault(p => p.IdPaquete == id);

            if (paquete == null)
            {
                return Json(new { precio = 0 }); // Si no se encuentra el paquete, devolver precio 0
            }

            // Devolver el precio del paquete en formato JSON
            return Json(new { precio = paquete.Precio.ToString("0") });
        }

        [AllowAnonymous]
        [HttpGet("api/dashboard/PaquetesMasReservados")]
        public IActionResult GetPaquetesMasReservados()
        {
            var datos = _context.Reservas
                .Include(r => r.DetallePaquetes) // Incluye la relación con la tabla paquetes
                .GroupBy(r => r.IdPaqueteNavigation.Nombre) // Agrupa las reservas por nombre de paquete
                .Select(grupo => new
                {
                    Paquete = grupo.Key, // Nombre de la Paquete
                    CantidadReservas = grupo.Count() // Número de reservas para ese Paquete
                })
                .OrderByDescending(x => x.CantidadReservas) // Ordena de mayor a menor cantidad de reservas
                .Take(5) // Devuelve los 5 paquetes más reservadas
                .ToList();

            return Ok(datos); // Devuelve un JSON con los paquetes y la cantidad de reservas
        }

        [AllowAnonymous]
        [HttpGet("api/dashboard/serviciosMasSolicitados")]
        public IActionResult GetServiciosMasSolicitados()
        {
            // Agrupa los servicios solicitados en las reservas y cuenta las ocurrencias
            var serviciosMasSolicitados = _context.Reservas
                .SelectMany(r => r.DetalleServicios) // Accede a los detalles de servicios en las reservas
                .GroupBy(ds => ds.IdServicioNavigation.Nombre) // Agrupa por el nombre del servicio
                .Select(grupo => new
                {
                    Servicio = grupo.Key,
                    Cantidad = grupo.Count()
                })
                .OrderByDescending(x => x.Cantidad) // Ordena por la cantidad en orden descendente
                .ToList();

            return Ok(serviciosMasSolicitados); // Devuelve un JSON con los servicios y las cantidades
        }

        [AllowAnonymous]
        [Route("api/dashboard/ReservasMasVendidas")]
        [HttpGet]
        public IActionResult GetReservasMasVendidas()
        {
            var reservasMasVendidas = _context.Reservas
                .GroupBy(r => r.IdPaqueteNavigation.Nombre) // Agrupar por nombre del paquete
                .Select(g => new
                {
                    Paquete = g.Key,        // Nombre del paquete
                    Cantidad = g.Count()    // Cantidad de reservas por paquete
                })
                .OrderByDescending(r => r.Cantidad) // Ordenar por las más vendidas
                .Take(5) // Limitar a los 5 paquetes más vendidos
                .ToList();

            return Ok(reservasMasVendidas);
        }

        [AllowAnonymous]
        [Route("api/dashboard/ReservasPorDia")]
        [HttpGet]
        public IActionResult GetReservasPorDia()
        {
            try
            {
                // Fecha inicial (últimos 10 días)
                var fechaInicio = DateTime.Now.AddDays(-10);

                // Cargar los datos desde la base de datos (sin usar ToString)
                var reservas = _context.Reservas
                    .Where(r => r.FechaReserva >= fechaInicio) // Filtrar en SQL
                    .ToList(); // Traer datos a memoria

                // Agrupar y procesar en memoria
                var reservasPorDia = reservas
                    .GroupBy(r => r.FechaReserva.Date)
                    .Select(g => new
                    {
                        Fecha = g.Key.ToString("yyyy-MM-dd"), // Formatear la fecha en memoria
                        Cantidad = g.Count()
                    })
                    .OrderBy(r => r.Fecha)
                    .ToList();

                return Ok(reservasPorDia);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
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

        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }
    }
}
