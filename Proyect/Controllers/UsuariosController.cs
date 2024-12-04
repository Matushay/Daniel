using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyect.Models;
using Proyect.Servicios;
using Proyect.ViewModel;
using Proyect.Validaciones.ValidacionesLuis; 

namespace Proyect.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly ProyectContext _context;
        private readonly SendGridEmailService _emailcreateService;

        public UsuariosController(ProyectContext context)
        {
            _context = context;
             var apiKey = "Aca clave api"; // Obtén tu API Key de SendGrid
            _emailcreateService = new SendGridEmailService(apiKey);
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
                    // Guardar al usuario en la base de datos
                    _context.Add(usuario);
                    await _context.SaveChangesAsync();

                    // Generar el código de restablecimiento
                    string codigoRestablecimiento = GenerarCodigoRestablecimiento();

                    // Guardar el código en el usuario (puedes agregar una columna "CodigoRestablecimiento" en tu base de datos)
                    usuario.CodigoRestablecimiento = codigoRestablecimiento;
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();

                    // Generar el enlace de restablecimiento de contraseña con el código
                    string enlaceRestablecimiento = Url.Action("ResetPassword", "Account", new { code = codigoRestablecimiento }, protocol: Request.Scheme);

                    // Contenido del correo
                    var subject = "Restablecimiento de contraseña";
                    var plainTextContent = $"Hola {usuario.Nombre},\n\nHaz clic en el siguiente enlace para restablecer tu contraseña:\n{enlaceRestablecimiento}\n\nSi no solicitaste este cambio, ignora este mensaje.";
                    var htmlContent = $@"
    <strong>Hola {usuario.Nombre}</strong>,<br><br>
    Haz clic en el siguiente botón para restablecer tu contraseña:<br><br>
    <a href='{enlaceRestablecimiento}' style='padding: 10px 20px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px;'>Restablecer mi contraseña</a><br><br>
    Si no solicitaste este cambio, por favor ignora este mensaje.
";

                    // Enviar el correo
                    await _emailcreateService.SendEmailAsync(usuario.CorreoElectronico, subject, plainTextContent, htmlContent);

                    // Redirigir al usuario a la página de lista o cualquier otra página
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

        [HttpGet]
        public IActionResult ResetPassword(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Código de restablecimiento de contraseña no válido.");
            }

            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Usuarios.SingleOrDefaultAsync(u => u.CodigoRestablecimiento == model.Code);
            if (user == null)
            {
                ModelState.AddModelError("", "Código de restablecimiento de contraseña no válido.");
                return View();
            }

            user.Contraseña = model.Password; // Aquí puedes agregar lógica para encriptar la contraseña
            user.CodigoRestablecimiento = null; // Opcional: limpiar el código de restablecimiento una vez utilizado
            await _context.SaveChangesAsync();

            return RedirectToAction("Login", "Account");
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


