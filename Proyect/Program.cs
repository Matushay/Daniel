using FluentValidation;
using FluentValidation.AspNetCore;
using MailKit;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
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

            // Agrega el DbContext al contenedor de servicios
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

            // Agregar servicios necesarios para la aplicaci�n
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configuraci�n de la tuber�a de solicitudes HTTP
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();  // Configuraci�n para entornos de producci�n
            }
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ProyectContext>(); context.Database.Migrate();
                if (!context.Usuarios.Any()) 
                { 
                    var adminUser = new Usuario 
                    {
                        TipoDocumento = "CC", 
                        Documento = "123456789",
                        Nombre = "Administrador", 
                        Apellido = "Principal", 
                        Celular = "1234567890", 
                        Direccion = "Direcci�n Admin", 
                        CorreoElectronico = "admin@proyect.com", 
                        Estado = true, 
                        Contrase�a = "Admin123.", 
                        FechaCreacion = DateTime.Now, 
                        IdRol = 1 
                    }; 
                    context.Usuarios.Add(adminUser); 
                    context.SaveChanges(); 
                } 
            }
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            // Configuraci�n de rutas para controladores MVC
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}

