using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Proyect.Models;
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

            // Registrar validadores de FluentValidation de toda la aplicación
            builder.Services.AddControllers()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<UsuarioValidator>());


            // Add services to the container.

            // Registrar validadores específicos si es necesario
            builder.Services.AddScoped<IValidator<Usuario>, UsuarioValidator>();
            builder.Services.AddScoped<IValidator<Role>, RolValidator>();
            builder.Services.AddScoped<IValidator<Permiso>, PermisoValidator>();

            // Agregar servicios necesarios para la aplicación
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configuración de la tubería de solicitudes HTTP
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();  // Configuración para entornos de producción
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // Configuración de rutas para controladores MVC
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();

        }
    }
}

