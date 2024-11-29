//using Microsoft.AspNetCore.Authorization;
//using Microsoft.EntityFrameworkCore;
//using Proyect01.Models;
//using Proyect01.Validaciones.validaciones_luis;
//using System.Security.Claims;

//namespace Proyect01.Validaciones.validaciones_luis
//{
//    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
//    {
//        private readonly IServiceProvider _serviceProvider;

//        public PermissionHandler(IServiceProvider serviceProvider)
//        {
//            _serviceProvider = serviceProvider;
//        }

//        protected override async Task HandleRequirementAsync(
//            AuthorizationHandlerContext context,
//            PermissionRequirement requirement)
//        {
//            // Obtener el ID del usuario autenticado
//            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

//            if (string.IsNullOrEmpty(userId))
//            {
//                context.Fail();
//                return;
//            }

//            // Crear un ámbito para resolver ProyectContext
//            using (var scope = _serviceProvider.CreateScope())
//            {
//                var dbContext = scope.ServiceProvider.GetRequiredService<ProyectContext>();

//                // Consultar permisos
//                var hasPermission = await dbContext.RolesPermisos
//                    .Include(rp => rp.IdRolNavigation)
//                    .Include(rp => rp.IdPermisoNavigation)
//                    .AnyAsync(rp =>
//                        rp.IdPermisoNavigation.NombrePermiso == requirement.Permission &&
//                        rp.IdRolNavigation.Usuarios.Any(u => u.IdUsuario.ToString() == userId));

//                if (hasPermission)
//                {
//                    context.Succeed(requirement);
//                }
//                else
//                {
//                    context.Fail();
//                }
//            }
//        }
//    }
//}
