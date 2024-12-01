using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyect.Models;
using Proyect.Validaciones.ValidacionesLuis;

namespace Proyect.Controllers
{
    //[Authorize] // Restringimos el acceso a usuarios autenticados
    //[Authorize] // Restringimos el acceso a usuarios autenticados
    public class RolesController : Controller
    {
        private readonly ProyectContext _context;

        public RolesController(ProyectContext context)
        {
            _context = context;
        }

        // GET: Roles
        //[Authorize(Policy = "ViewRoles")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Roles.ToListAsync());
        }

        // GET: Roles/Details/5
        //[Authorize(Policy = "ViewRoles")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .Include(r => r.RolesPermisos)
                .ThenInclude(rp => rp.IdPermisoNavigation)
                .FirstOrDefaultAsync(m => m.IdRol == id);

            if (role == null)
            {
                return NotFound();
            }
           

            return View(role);

        }

        // GET: Roles/Create
        //[Authorize(Policy = "ManageRoles")]
        public IActionResult Create()
        {
            ViewData["Permisos"] = _context.Permisos.ToList(); // Resuelve la lista de permisos sincrónicamente
            return View();
        }

        // POST: Roles/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Policy = "ManageRoles")]

        public IActionResult Create([Bind("IdRol,NombreRol,Descripcion")] Role role, List<int> selectedPermisos)
        {
            // Obtener nombres existentes para pasar al validador
            var nombresExistentes = _context.Roles.Select(r => r.NombreRol).ToList();
            var validator = new RolValidator(nombresExistentes);
            var validationResult = validator.Validate(role);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.ErrorMessage);
                }

                // Recargar permisos si hay errores
                ViewData["Permisos"] = _context.Permisos.ToList();
                return View(role);
            }

            // Verificar permisos seleccionados
            if (selectedPermisos != null && selectedPermisos.Any())
            {
                var permisosInvalidos = selectedPermisos
                    .Where(id => !_context.Permisos.Any(p => p.IdPermiso == id))
                    .ToList();

                if (permisosInvalidos.Any())
                {
                    ModelState.AddModelError("selectedPermisos", "Uno o más permisos seleccionados no son válidos.");
                    ViewData["Permisos"] = _context.Permisos.ToList();
                    return View(role);
                }
            }

            // Guardar rol y sus permisos en la base de datos
            _context.Add(role);
            _context.SaveChanges();

            if (selectedPermisos != null && selectedPermisos.Any())
            {
                var rolesPermisos = selectedPermisos.Select(id => new RolesPermiso
                {
                    IdRol = role.IdRol,
                    IdPermiso = id
                });

                _context.AddRange(rolesPermisos);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
        // GET: Roles/Edit/5
        //[Authorize(Policy = "ManageRoles")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Obtener el rol con sus permisos actuales
            var role = await _context.Roles
                .Include(r => r.RolesPermisos)
                .ThenInclude(rp => rp.IdPermisoNavigation)
                .FirstOrDefaultAsync(m => m.IdRol == id);

            if (role == null)
            {
                return NotFound();
            }

            // Pasar la lista de permisos disponibles a ViewData
            ViewData["Permisos"] = await _context.Permisos.ToListAsync();

            // Pasar los IDs de los permisos asociados al rol a ViewData
            ViewData["SelectedPermisos"] = role.RolesPermisos.Select(rp => rp.IdPermiso).ToList();

            return View(role);
        }


        // POST: Roles/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Policy = "ManageRoles")]
        public async Task<IActionResult> Edit(int id, [Bind("IdRol,NombreRol,Descripcion")] Role role, List<int> selectedPermisos)
        {
            if (id != role.IdRol)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Actualizar los datos del rol
                    _context.Update(role);
                    await _context.SaveChangesAsync();

                    // Actualizar los permisos asociados al rol
                    var permisosExistentes = _context.RolesPermisos.Where(rp => rp.IdRol == id).ToList();
                    _context.RolesPermisos.RemoveRange(permisosExistentes);

                    // Asignar permisos
                    if (selectedPermisos != null && selectedPermisos.Count > 0)
                    {
                        foreach (var permisoId in selectedPermisos)
                        {
                            var rolesPermiso = new RolesPermiso
                            {
                                IdRol = role.IdRol,
                                IdPermiso = permisoId
                            };
                            _context.RolesPermisos.Add(rolesPermiso);
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoleExists(role.IdRol))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            // En caso de error, devolver los datos necesarios para la vista
            ViewData["Permisos"] = await _context.Permisos.ToListAsync();
            ViewData["SelectedPermisos"] = selectedPermisos;

            return View(role);
        }


        // GET: Roles/Delete/5
        //[Authorize(Policy = "ManageRoles")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .Include(r => r.Usuarios) // Carga los usuarios asociados al rol
                .FirstOrDefaultAsync(m => m.IdRol == id);

            if (role == null)
            {
                return NotFound();
            }

            // Si el rol tiene usuarios asociados, mostrar advertencia
            if (role.Usuarios.Any())
            {
                TempData["Error"] = "Este rol tiene usuarios asociados y no se puede eliminar.";
                return RedirectToAction(nameof(Index));
            }

            return View(role); // Pasar el rol a la vista
        }



        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var role = await _context.Roles
                    .Include(r => r.Usuarios) // Incluye los usuarios para verificar relaciones
                    .FirstOrDefaultAsync(r => r.IdRol == id);

                if (role == null)
                {
                    return NotFound();
                }

                // Verificar si el rol tiene usuarios asociados
                if (role.Usuarios.Any())
                {
                    TempData["Error"] = "Este rol tiene usuarios asociados y no se puede eliminar.";
                    return RedirectToAction(nameof(Index));
                }

                // Eliminar relaciones con RolesPermisos
                var rolesPermisosExistentes = _context.RolesPermisos.Where(rp => rp.IdRol == id);
                _context.RolesPermisos.RemoveRange(rolesPermisosExistentes);

                // Eliminar el rol
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Rol eliminado correctamente.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }


            return RedirectToAction(nameof(Index));
        }

        // Método auxiliar para verificar si el rol existe
        private bool RoleExists(int id)
        {
            return _context.Roles.Any(e => e.IdRol == id);
        }  }

}