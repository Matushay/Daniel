
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyect.Models;
using Proyect.Servicios;
using Proyect.Validaciones.ValidacionesLuis; // Importar las validaciones

namespace Proyect.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly ProyectContext _context;
        private readonly IEmailService _emailService;

        public UsuariosController(ProyectContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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

        // Método para generar una contraseña aleatoria
        private string GenerarContraseñaAleatoria(int longitud = 8)
        {
            const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(caracteres, longitud)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Generar un código de restablecimiento único
        private string GenerarCodigoRestablecimiento()
        {
            return Guid.NewGuid().ToString("N");
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdUsuario,TipoDocumento,Documento,Nombre,Apellido,Celular,Direccion,CorreoElectronico,FechaCreacion,IdRol")] Usuario usuario)
        {
            if (usuario == null)
            {
                return BadRequest("El objeto usuario es nulo.");
            }

            // Validación: Verificar si el rol está activo
            var rol = _context.Roles.FirstOrDefault(r => r.IdRol == usuario.IdRol);
            if (rol != null && !rol.Estado)
            {
                ModelState.AddModelError("IdRol", "El rol seleccionado está inactivo y no puede ser asignado.");
            }

            // Validación manual con el validador
            var validator = new UsuarioValidator(_context);
            var validationResult = validator.Validate(usuario);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
            }

            if (ModelState.IsValid)
            {
                // Generar y asignar contraseña aleatoria al usuario
                usuario.Contraseña = GenerarContraseñaAleatoria();

                // Asegúrate de que la contraseña no sea nula
                if (string.IsNullOrEmpty(usuario.Contraseña))
                {
                    ModelState.AddModelError("Contraseña", "No se pudo generar una contraseña para el usuario.");
                }
                else
                {
                    _context.Add(usuario);
                    await _context.SaveChangesAsync();

                    // Verificar que el servicio de correo electrónico y la dirección de correo electrónico del usuario no sean nulos
                    if (_emailService != null && !string.IsNullOrEmpty(usuario.CorreoElectronico))
                    {
                        // Enviar correo electrónico con la contraseña generada
                        string resetPasswordUrl = Url.Action("ResetPassword", "Account", new { email = usuario.CorreoElectronico }, Request.Scheme);
                        string emailBody = $"¡Bienvenido a Medellin Salvaje!<br>" +
                                           $"Tu contraseña temporal es: <strong>{usuario.Contraseña}</strong><br>" +
                                           $"Haz clic <a href='{resetPasswordUrl}'>aquí</a> para cambiar tu contraseña.";

                        try
                        {
                            await _emailService.SendEmailAsync(
                                usuario.CorreoElectronico,
                                "Bienvenido a Medellin Salvaje",
                                emailBody
                            );
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", $"Error al enviar el correo electrónico: {ex.Message}");
                        }
                    }
                    else
                    {
                        // Manejar el caso en que _emailService o CorreoElectronico sea nulo
                        ModelState.AddModelError("", "No se pudo enviar el correo electrónico. Verifica la configuración del servicio de correo y la dirección de correo electrónico del usuario.");
                    }

                    return RedirectToAction(nameof(Index));
                }
            }

            ViewBag.IdRol = new SelectList(_context.Roles, "IdRol", "NombreRol", usuario.IdRol);
            return View(usuario);
        }


        // GET: Usuarios/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var usuario = _context.Usuarios.Find(id);
            if (usuario == null)
            {
                return NotFound();
            }

            ViewBag.IdRol = new SelectList(_context.Roles, "IdRol", "NombreRol", usuario.IdRol);
            return View(usuario);
        }


        // POST: Usuarios/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdUsuario,TipoDocumento,Documento,Nombre,Apellido,Celular,Direccion,CorreoElectronico,FechaCreacion,IdRol")] Usuario usuario)
        {
            if (id != usuario.IdUsuario)
            {
                return NotFound();
            }

            // Validación: Verificar si el rol está activo
            var rol = _context.Roles.FirstOrDefault(r => r.IdRol == usuario.IdRol);
            if (rol != null && !rol.Estado)
            {
                ModelState.AddModelError("IdRol", "El rol seleccionado está inactivo y no puede ser asignado.");
            }

            // Validación manual con el validador
            var validator = new UsuarioValidator(_context);
            var validationResult = validator.Validate(usuario);

            if (!validationResult.IsValid)
            {

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
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Usuarios.Any(e => e.IdUsuario == usuario.IdUsuario))
                    {
                        return NotFound();
                    }
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }


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


