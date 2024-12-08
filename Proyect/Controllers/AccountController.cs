using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyect.Models;
using Proyect.ViewModel;
using System.Security.Claims;
using Proyect.Servicios;
using Microsoft.AspNetCore.Authorization;

namespace Proyect.Controllers
{
    public class AccountController : Controller
    {
        private readonly ProyectContext _context;
        private readonly IEmailService _emailService;

        public AccountController(ProyectContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Buscar al usuario con el correo electrónico y contraseña
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.CorreoElectronico == model.CorreoElectronico && u.Contraseña == model.Contraseña);

                if (usuario != null)
                {
                    // Crear la lista de claims básicos del usuario
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.CorreoElectronico)
            };

                    // Obtener los permisos asociados al rol del usuario
                    var permisos = await _context.RolesPermisos
                        .Where(rp => rp.IdRol == usuario.IdRol)
                        .Select(rp => rp.IdPermisoNavigation.NombrePermiso)
                        .ToListAsync();

                    // Agregar los permisos como claims
                    foreach (var permiso in permisos)
                    {
                        claims.Add(new Claim("Permission", permiso));
                    }

                    // Crear una identidad con los claims obtenidos
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Iniciar sesión con los claims
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    Console.WriteLine("Usuario autenticado: " + usuario.Nombre);

                    // Redirigir a la página principal o a la vista correspondiente
                    return RedirectToAction("Index", "Home");
                }

                // Si no se encuentra el usuario, mostrar un mensaje de error
                ModelState.AddModelError("", "Correo electronico o contraseña incorrectos.");
            }

            // Si la validación falla, retornar la vista de login
            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Por favor, ingrese su correo electrónico.");
                return View();
            }

            var user = await _context.Usuarios.SingleOrDefaultAsync(u => u.CorreoElectronico == email);
            if (user == null)
            {
                ModelState.AddModelError("", "No se encontró ningún usuario con ese correo electrónico.");
                return View();
            }

            var resetCode = Guid.NewGuid().ToString();
            user.CodigoRestablecimiento = resetCode;
            await _context.SaveChangesAsync();

            var callbackUrl = Url.Action("ResetPassword", "Account", new { code = resetCode }, protocol: HttpContext.Request.Scheme);
            var emailBody = callbackUrl;

            await _emailService.SendEmailAsync(email, "Restablecer Contraseña", emailBody);

            ViewBag.Message = "Se ha enviado un enlace para restablecer su contraseña a su correo electrónico.";
            return View();
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

            TempData["SuccessMessage"] = "La contraseña se ha restablecido con éxito.";
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }
    }
}
