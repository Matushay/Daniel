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
    [Authorize(Policy = "AccederClientes")]
    public class ClientesController : Controller
    {
        private readonly ProyectContext _context;

        public ClientesController(ProyectContext context)
        {
            _context = context;
        }

        // GET: Clientes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clientes.ToListAsync());
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.IdCliente == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCliente,TipoDocumento,Documento,Nombre,Apellido,Direccion,Celular,CorreoElectronico,Estado")] Cliente cliente)
        {
            // Verificar si el correo electrónico ya existe
            var clienteExistente = await _context.Clientes
                .FirstOrDefaultAsync(u => u.CorreoElectronico == cliente.CorreoElectronico);

            if (clienteExistente != null)
            {
                ModelState.AddModelError("CorreoElectronico", "Este correo electrónico ya está registrado.");
                return View(cliente); // Regresar a la vista con el error
            }

            // Verificar si el documento ya existe
            var documentoExistente = await _context.Clientes
                .FirstOrDefaultAsync(u => u.Documento == cliente.Documento);

            if (documentoExistente != null)
            {
                ModelState.AddModelError("Documento", "Este número de documento ya está registrado.");
                return View(cliente); // Regresar a la vista con el error
            }

            if (ModelState.IsValid)
            {
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "El cliente se creó correctamente";
                return RedirectToAction(nameof(Index));
            }

            return View(cliente);
        }
        // POST: Clientes/Create desde el modal (parcial)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFromModal([Bind("IdCliente,TipoDocumento,Documento,Nombre,Apellido,Direccion,Celular,CorreoElectronico,Estado")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el documento ya existe
                    var clienteExistente = await _context.Clientes
                        .FirstOrDefaultAsync(c => c.Documento == cliente.Documento);

                    if (clienteExistente != null)
                    {
                        // Si el cliente con el mismo documento ya existe, devolver un mensaje de error
                        ModelState.AddModelError("Documento", "Ya existe un cliente con ese documento.");
                        return Json(new { success = false, message = "Ya existe un cliente con ese documento.", field = "Documento" });
                    }

                    // Verificar si el correo ya está registrado
                    var correoExistente = await _context.Clientes
                        .FirstOrDefaultAsync(c => c.CorreoElectronico == cliente.CorreoElectronico);

                    if (correoExistente != null)
                    {
                        ModelState.AddModelError("CorreoElectronico", "Ya existe un cliente con ese correo.");
                        return Json(new { success = false, message = "Ya existe un cliente con ese correo.", field = "CorreoElectronico" });
                    }

                    // Agregar el cliente al contexto de la base de datos
                    _context.Add(cliente);
                    await _context.SaveChangesAsync();

                    // Retornar el ID del cliente creado en la respuesta JSON
                    return Json(new { success = true, clientId = cliente.IdCliente });
                }
                catch (Exception ex)
                {
                    var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    return Json(new { success = false, message = $"Error al crear el cliente: {innerMessage}" });
                }
            }

            // Si el modelo no es válido, devolver un mensaje detallado
            return Json(new { success = false, message = "Datos inválidos, por favor revise los campos del formulario." });
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCliente,TipoDocumento,Documento,Nombre,Apellido,Direccion,Celular,CorreoElectronico,Estado")] Cliente cliente)
        {
            if (id != cliente.IdCliente)
            {
                return NotFound();
            }

            // Verificar si el correo electrónico ya existe
            var clienteExistente = await _context.Clientes
                .FirstOrDefaultAsync(u => u.CorreoElectronico == cliente.CorreoElectronico);

            if (clienteExistente != null)
            {
                ModelState.AddModelError("CorreoElectronico", "Este correo electrónico ya está registrado.");
                return View(cliente); // Regresar a la vista con el error
            }

            // Verificar si el documento ya existe
            var documentoExistente = await _context.Clientes
                .FirstOrDefaultAsync(u => u.Documento == cliente.Documento);

            if (documentoExistente != null)
            {
                ModelState.AddModelError("Documento", "Este número de documento ya está registrado.");
                return View(cliente); // Regresar a la vista con el error
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.IdCliente))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessMessage"] = "El cliente editado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.IdCliente == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "El cliente se elimino correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ActualizarEstado(int id, bool estado)
        {
            var cliente  = _context.Clientes.Find(id);
            if (cliente != null)
            {
                cliente.Estado = estado;
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Cliente no encontrada." });
        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.IdCliente == id);
        }
        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }
    }
}
