using FluentValidation.AspNetCore;
using MailKit;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Proyect.Models;
using Proyect.Servicios;
using Proyect.Validaciones;

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
                    options.LoginPath = "/Account/Login"; // Redirige a la página de inicio de sesión si no está autenticado
                    options.AccessDeniedPath = "/Account/AccessDenied";
                });

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
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
                        Direccion = "Dirección Admin", 
                        CorreoElectronico = "admin@proyect.com", 
                        Estado = true, 
                        Contraseña = "Admin123.", 
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

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}

