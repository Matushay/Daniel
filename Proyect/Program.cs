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

            // Configurar políticas de autorización
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AccederDashboard", policy => policy.RequireClaim("Permission", "Acceder Dashboard"));
                options.AddPolicy("AccederUsuarios", policy => policy.RequireClaim("Permission", "Acceder Usuarios"));
                options.AddPolicy("AccederClientes", policy => policy.RequireClaim("Permission", "Acceder Clientes"));
                options.AddPolicy("AccederRoles", policy => policy.RequireClaim("Permission", "Acceder Roles"));
                options.AddPolicy("AccederMuebles", policy => policy.RequireClaim("Permission", "Acceder Muebles"));
                options.AddPolicy("AccederTipoMuebles", policy => policy.RequireClaim("Permission", "Acceder Tipo de Muebles"));
                options.AddPolicy("AccederHabitaciones", policy => policy.RequireClaim("Permission", "Acceder Habitaciones"));
                options.AddPolicy("AccederTipoHabitaciones", policy => policy.RequireClaim("Permission", "Acceder Tipo de Habitaciones"));
                options.AddPolicy("AccederServicios", policy => policy.RequireClaim("Permission", "Acceder Servicios"));
                options.AddPolicy("AccederPaquetes", policy => policy.RequireClaim("Permission", "Acceder Paquetes"));
                options.AddPolicy("AccederReservas", policy => policy.RequireClaim("Permission", "Acceder Reservas"));
                options.AddPolicy("AccederAbonos", policy => policy.RequireClaim("Permission", "Acceder Abonos"));
            });
        

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
