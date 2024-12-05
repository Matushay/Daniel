using FluentValidation;
using FluentValidation.AspNetCore;
using MailKit;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Proyect.Data;
using Proyect.Models;
using Proyect.Servicios;
using Proyect.Validaciones;
using Proyect.Validaciones.ValidacionesLuis;

namespace Proyect
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Agregar el DbContext al contenedor de servicios
            builder.Services.AddDbContext<ProyectContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ProyectConnection")));

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddControllers().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Program>());

            // Configurar EmailSettings
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

            // Registrar el servicio de Email
            builder.Services.AddTransient<Proyect.Servicios.IEmailService, Proyect.Servicios.EmailService>();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login"; // Redirige a la p�gina de inicio de sesi�n si no est� autenticado
                    options.AccessDeniedPath = "/Account/AccessDenied";
                });

            // Add services to the container.

            // Registrar validadores espec�ficos si es necesario
            builder.Services.AddScoped<IValidator<Usuario>, UsuarioValidator>();
            builder.Services.AddScoped<IValidator<Role>, RolValidator>();
            builder.Services.AddScoped<IValidator<Permiso>, PermisoValidator>();

            // Agregar servicios necesarios para la aplicaci�n, como los controladores con vistas
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configuraci�n de la tuber�a de solicitudes HTTP
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();  // Configuraci�n para entornos de producci�n
            }

            //using (var scope = app.Services.CreateScope())
            //{
            //    var context = scope.ServiceProvider.GetRequiredService<ProyectContext>();

            //    // Verifica si el usuario ya existe
            //    if (!context.Usuarios.Any(u => u.CorreoElectronico == "unknownmurder569@gmail.com"))
            //    {
            //        var adminUser = new Usuario
            //        {
            //            TipoDocumento = "CC",
            //            Documento = "123456789",
            //            Nombre = "Administrador",
            //            Apellido = "Principal",
            //            Celular = "1234567890",
            //            Direccion = "Direccion Admin",
            //            CorreoElectronico = "unknownmurder569@gmail.com",
            //            Estado = true,
            //            Contraseña = "Admin123.", // Asegúrate de que esto se encripte si usas autenticación real.
            //            FechaCreacion = DateTime.Now,
            //            IdRol = 1
            //        };
            //        context.Usuarios.Add(adminUser);
            //        context.SaveChanges();
            //    }
            //}

            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            // Configuraci�n de rutas para controladores MVC
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            SeedData.Initialize(app.Services);

            app.Run();
        }
    }
}
