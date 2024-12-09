using Microsoft.EntityFrameworkCore;
using Proyect.Models;

namespace Proyect.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ProyectContext>();
                // Crear el rol de SuperAdmin si no existe
                if (!context.Roles.Any(r => r.NombreRol == "SuperAdmin"))
                {
                    context.Roles.Add(new Role { NombreRol = "SuperAdmin" });
                    context.SaveChanges();
                }

                // Crear el usuario SuperAdmin si no existe
                if (!context.Usuarios.Any(u => u.CorreoElectronico == "superadminmedellinsalvaje@gmail.com"))
                {
                    var roleId = context.Roles.First(r => r.NombreRol == "SuperAdmin").IdRol;
                    var superAdmin = new Usuario
                    {
                        TipoDocumento = "CC", // O el tipo de documento adecuado
                        Documento = "00000000",
                        Nombre = "Administrador",
                        Apellido = "Admin",
                        Celular = "1234567890",
                        Direccion = "Admin Street 123",
                        CorreoElectronico = "medellinsalvaje@gmail.com",
                        Contraseña = "SuperAdmin123.*", // Asegúrate de encriptar la contraseña
                        Estado = true,
                        FechaCreacion = DateTime.Now,
                        IdRol = roleId
                    };

                    context.Usuarios.Add(superAdmin);
                    context.SaveChanges();
                }
            }
        }
    }
}
