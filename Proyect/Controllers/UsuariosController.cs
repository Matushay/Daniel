
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyect.Models;
using Proyect.Validaciones.ValidacionesLuis; // Importar las validaciones

namespace Proyect.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly ProyectContext _context;

        public UsuariosController(ProyectContext context)
        {
            _context = context;
            
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var proyectContext = _context.Usuarios.Include(u => u.IdRolNavigation);
            return View(await proyectContext.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(m => m.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            ViewBag.IdRol = new SelectList(_context.Roles, "IdRol", "NombreRol");
            return View();
        }

        // POST: Usuarios/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdUsuario,TipoDocumento,Documento,Nombre,Apellido,Celular,Direccion,CorreoElectronico,Estado,Contraseña,FechaCreacion,IdRol")] Usuario usuario)
        {
            // Validación manual con el validador
            var validator = new UsuarioValidator(_context); // Crear instancia del validador
            var validationResult = validator.Validate(usuario); // Validar de forma síncrona

            if (!validationResult.IsValid)
            {
                // Si la validación falla, agregar los errores al ModelState
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(usuario);
                _context.SaveChanges(); // Operación síncrona para mantener consistencia
                return RedirectToAction(nameof(Index));
            }

            // Si hay errores, recargar los roles y devolver la vista
            ViewBag.IdRol = new SelectList(_context.Roles, "IdRol", "NombreRol", usuario.IdRol);
            return View(usuario);
        }


        // GET: Usuarios/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            { return NotFound();}
             
       
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null)

                return NotFound();

            ViewBag.IdRol = new SelectList(_context.Roles, "IdRol", "NombreRol", usuario.IdRol);
            return View(usuario);
        }

        // POST: Usuarios/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdUsuario,TipoDocumento,Documento,Nombre,Apellido,Celular,Direccion,CorreoElectronico,Estado,Contraseña,FechaCreacion,IdRol")] Usuario usuario)
        {
            if (id != usuario.IdUsuario)
                return NotFound();

            // Validación manual con el validador
            var validator = new UsuarioValidator(_context); // Crear instancia del validador
            var validationResult = validator.Validate(usuario); // Validar de forma síncrona

            if (!validationResult.IsValid)
            {
                // Si la validación falla, agregar los errores al ModelState
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    _context.SaveChanges(); // Operación síncrona
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Usuarios.Any(e => e.IdUsuario == usuario.IdUsuario))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            // Si hay errores, recargar los roles y devolver la vista
            ViewBag.IdRol = new SelectList(_context.Roles, "IdRol", "NombreRol", usuario.IdRol);
            return View(usuario);
        }


            // POST: Usuarios/ActualizarEstado
            [HttpPost]
            public async Task<IActionResult> ActualizarEstado(int id, bool estado)
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario != null)
                {
                    usuario.Estado = estado;
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                    return Ok(); // Responde con éxito
                }
                return BadRequest(); // En caso de error
            }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(m => m.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == id);
        }
    }
}


