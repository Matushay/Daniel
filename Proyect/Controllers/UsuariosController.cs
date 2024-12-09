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
    [Authorize(Policy = "AccederUsuarios")]
    public class UsuariosController : Controller
    {
        private readonly ProyectContext _context;
        private readonly SendGridEmailService _emailcreateService;

        public UsuariosController(ProyectContext context)
        {
            _context = context;
            var apiKey = "Clave api para enviar correos"; // API Key de SendGrid
            _emailcreateService = new SendGridEmailService(apiKey);
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            // Filtrar los usuarios para excluir al superadministrador
            var proyectContext = _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .Where(u => !u.IdRolNavigation.NombreRol.Equals("SuperAdmin"));

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

            // Verificar si el correo electrónico ya existe
            var usuarioExistente = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.CorreoElectronico == usuario.CorreoElectronico);

            if (usuarioExistente != null)
            {
                ModelState.AddModelError("CorreoElectronico", "Este correo electrónico ya está registrado.");
                return View(usuario); // Regresar a la vista con el error
            }

            // Verificar si el documento ya existe
            var documentoExistente = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Documento == usuario.Documento);

            if (documentoExistente != null)
            {
                ModelState.AddModelError("Documento", "Este número de documento ya está registrado.");
                return View(usuario); // Regresar a la vista con el error
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
                    // Contenido del correo HTML estilizado
                    var htmlContent = $@"
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f9f9f9;
            margin: 0;
            padding: 0;
        }}
        .email-container {{
            max-width: 600px;
            margin: 40px auto;
            background-color: #ffffff;
            border: 1px solid #e0e0e0;
            border-radius: 8px;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
            padding: 20px;
            text-align: center;
        }}
        .logo-container {{
            margin-bottom: 20px;
        }}
        .logo {{
            max-width: 150px;
        }}
        .email-header {{
            font-size: 24px;
            font-weight: bold;
            color: #333333;
            margin-bottom: 20px;
        }}
        .email-content {{
            font-size: 16px;
            color: #555555;
            line-height: 1.6;
            margin-bottom: 30px;
        }}
        .btn {{
            display: inline-block;
            padding: 12px 20px;
            font-size: 16px;
            color: white;
            background-color: #007bff;
            text-decoration: none;
            border-radius: 4px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
            transition: background-color 0.3s ease;
        }}
        .btn:hover {{
            background-color: #0056b3;
        }}
        .footer {{
            font-size: 12px;
            color: #888888;
            margin-top: 20px;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='logo-container'>
            <img src='https://imgur.com/a/FFbmozo' alt='Logo' class='logo'/>
        </div>
        <div class='email-header'>Recuperación de contraseña</div>
        <div class='email-content'>
            Hola <strong>{usuario.Nombre}</strong>,<br><br>
            Por favor, restablezca su contraseña haciendo clic en el botón de abajo:
        </div>
        <a href='{enlaceRestablecimiento}' class='btn'>Restablecer Contraseña</a>
        <div class='email-content' style='margin-top: 20px;'>
            Si no solicitaste este cambio, por favor ignora este mensaje.
        </div>
        <div class='footer'>© 2024 Medellin Salvaje. Todos los derechos reservados.</div>
    </div>
</body>
</html>";



                    // Enviar el correo
                    await _emailcreateService.SendEmailAsync(usuario.CorreoElectronico, subject, plainTextContent, htmlContent);

                    // Redirigir al usuario a la página de lista o cualquier otra página
                    TempData["SuccessMessage"] = "El usuario se creó correctamente";
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

            usuario.EsEdicion = true;
            ViewBag.IdRol = new SelectList(_context.Roles, "IdRol", "NombreRol", usuario.IdRol);
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdUsuario,TipoDocumento,Documento,Nombre,Apellido,Celular,Direccion,CorreoElectronico,FechaCreacion,IdRol")] Usuario usuario)
        {
            if (id != usuario.IdUsuario)
            {
                return NotFound();
            }

            // Verificar si el correo electrónico ya existe
            var usuariocorreoExistente = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.CorreoElectronico == usuario.CorreoElectronico);

            if (usuariocorreoExistente != null)
            {
                ModelState.AddModelError("CorreoElectronico", "Este correo electrónico ya está registrado.");
                return View(usuario); // Regresar a la vista con el error
            }

            // Verificar si el documento ya existe
            var documentoExistente = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Documento == usuario.Documento);

            if (documentoExistente != null)
            {
                ModelState.AddModelError("Documento", "Este número de documento ya está registrado.");
                return View(usuario); // Regresar a la vista con el error
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
                    var usuarioExistente = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.IdUsuario == id);
                    var rolSuperAdmin = await _context.Roles.FirstOrDefaultAsync(r => r.NombreRol == "SuperAdmin");

                    if (usuarioExistente != null && usuarioExistente.IdRol == rolSuperAdmin?.IdRol)
                    {
                        // No permitir editar el superadministrador
                        ModelState.AddModelError("", "No se puede editar el usuario superadministrador.");
                        ViewBag.IdRol = new SelectList(_context.Roles, "IdRol", "NombreRol", usuario.IdRol);
                        return View(usuario);
                    }

                    // Mantener la contraseña existente si no se proporciona una nueva
                    if (string.IsNullOrEmpty(usuario.Contraseña))
                    {
                        usuario.Contraseña = usuarioExistente.Contraseña;
                    }
                    else
                    {
                        // Aquí puedes agregar la lógica para encriptar la contraseña si es necesario
                        usuario.Contraseña = (usuario.Contraseña);  // Reemplaza esto por tu método de encriptación
                    }

                    _context.Entry(usuario).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Usuarios.Any(e => e.IdUsuario == usuario.IdUsuario))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessMessage"] = "El usuario se editó correctamente";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.IdRol = new SelectList(_context.Roles, "IdRol", "NombreRol", usuario.IdRol);
            return View(usuario);
        }


        [HttpPost]
        public IActionResult ActualizarEstado(int id, bool estado)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario != null)
            {
                usuario.Estado = estado;
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Usuario no encontrada." });
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

            var superAdminRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.NombreRol == "SuperAdmin");

            //if (superAdminRole != null && usuario.IdRol == superAdminRole.IdRol)
            //{
            //    TempData["ErrorMessage"] = "No se puede eliminar el usuario superadministrador.";
            //} 

            return View(usuario);
        }

        [Route("api/dashboard/usuariosPorRol")]
        public IActionResult GetUsuariosPorRol(Usuario usuario)
        {
            var datos = _context.Usuarios
                .Include(u => u.IdRolNavigation) // Incluye explícitamente la navegación
                .GroupBy(u => u.IdRolNavigation.NombreRol)
                .Select(grupo => new
                {
                    Rol = grupo.Key,
                    Cantidad = grupo.Count()
                })
                .ToList();


            return Ok(datos); // Devuelve un JSON con los roles y la cantidad de usuarios
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }
            var superAdminRole = await _context.Roles.
                FirstOrDefaultAsync(r => r.NombreRol == "SuperAdmin");

            if (superAdminRole != null && usuario.IdRol == superAdminRole.IdRol)
            {
                TempData["ErrorMessage"] = "No se puede eliminar el usuario superadministrador.";

                return View(usuario);
            }
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "El usuario eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == id);
        }

        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }
    }
}


