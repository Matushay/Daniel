using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyect.Models;
using Proyect.Validaciones.ValidacionesLuis;

namespace Proyect.Controllers
{
    [Authorize]
    [Authorize(Policy = "AccederRoles")]

    public class RolesController : Controller
    {
        private readonly ProyectContext _context;

        public RolesController(ProyectContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _context.Roles
                .Include(r => r.RolesPermisos)
                .ThenInclude(rp => rp.IdPermisoNavigation)
                .Where(r => !r.NombreRol.Equals("SuperAdmin"))
                .ToListAsync();

            return View(roles);
        }

        // GET: Roles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rol = await _context.Roles
                .Include(r => r.RolesPermisos)
                .ThenInclude(rp => rp.IdPermisoNavigation)
                .FirstOrDefaultAsync(m => m.IdRol == id);

            if (rol == null)
            {
                return NotFound();
            }

            return View(rol);
        }


        // GET: Roles/Create
        public IActionResult Create()
        {
            ViewData["Permisos"] = _context.Permisos.ToList();
            return View();
        }


        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdRol,NombreRol,Descripcion,Estado")] Role role, int[] selectedPermisos)
        {
            if (ModelState.IsValid)
            {
                _context.Add(role);
                await _context.SaveChangesAsync();

                // Añadir los permisos seleccionados al rol
                if (selectedPermisos != null)
                {
                    foreach (var permisoId in selectedPermisos)
                    {
                        _context.RolesPermisos.Add(new RolesPermiso { IdRol = role.IdRol, IdPermiso = permisoId });
                    }
                    await _context.SaveChangesAsync();
                }
                TempData["SuccessMessage"] = "El rol se creo correctamente";
                return RedirectToAction(nameof(Index));
            }

            ViewData["Permisos"] = _context.Permisos.ToList();
            return View(role);
        }

        // Método para cambiar el estado (Activo/Inactivo)
        [HttpPost]
        public IActionResult ActualizarEstado(int idRol, bool estado)
        {
            // Buscar el Rol (o el objeto que corresponda) en la base de datos
            var rol = _context.Roles.FirstOrDefault(r => r.IdRol == idRol);

            if (rol == null)
            {
                return NotFound(); // Si no se encuentra el rol, devolvemos un error
            }

            // Actualizar el estado del rol
            rol.Estado = estado;

            // Guardar los cambios en la base de datos
            _context.SaveChanges();

            // Retornar una respuesta exitosa
            return Json(new { success = true });
        }
    
        // GET: Roles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rol = await _context.Roles
                .Include(r => r.RolesPermisos)
                .ThenInclude(rp => rp.IdPermisoNavigation)
                .FirstOrDefaultAsync(m => m.IdRol == id);

            if (rol == null)
            {
                return NotFound();
            }

            ViewData["Permisos"] = await _context.Permisos.ToListAsync();
            ViewData["SelectedPermisos"] = rol.RolesPermisos.Select(p => p.IdPermiso.Value).ToList();
            return View(rol);
        }

        // POST: Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdRol,NombreRol,Descripcion,Estado")] Role role, int[] selectedPermisos)
        {
            if (id != role.IdRol)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingRole = await _context.Roles
                        .Include(r => r.RolesPermisos)
                        .FirstOrDefaultAsync(r => r.IdRol == id);

                    if (existingRole != null)
                    {
                        existingRole.NombreRol = role.NombreRol;
                        existingRole.Descripcion = role.Descripcion;
                        existingRole.Estado = role.Estado;

                        // Eliminar los permisos existentes
                        _context.RolesPermisos.RemoveRange(existingRole.RolesPermisos);

                        // Añadir los nuevos permisos
                        if (selectedPermisos != null)
                        {
                            foreach (var permisoId in selectedPermisos)
                            {
                                _context.RolesPermisos.Add(new RolesPermiso { IdRol = id, IdPermiso = permisoId });
                            }
                        }

                        await _context.SaveChangesAsync();
                    }
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
                TempData["SuccessMessage"] = "El rol se edito correctamente";
                return RedirectToAction(nameof(Index));
            }

            ViewData["Permisos"] = await _context.Permisos.ToListAsync();
            ViewData["SelectedPermisos"] = selectedPermisos.ToList();
            return View(role);
        }

        // GET: Roles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .Include(r => r.Usuarios)
                .FirstOrDefaultAsync(m => m.IdRol == id);

            if (role == null)
            {
                return NotFound();
            }


            if (role.Usuarios.Any())
            {
                TempData["Error"] = "Este rol tiene usuarios asociados y no se puede eliminar.";
                return RedirectToAction(nameof(Index));
            }

            return View(role);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var role = await _context.Roles
                    .Include(r => r.Usuarios)
                    .FirstOrDefaultAsync(r => r.IdRol == id);

                if (role == null)
                {
                    return NotFound();
                }


                if (role.Usuarios.Any())
                {
                    TempData["Error"] = "Este rol tiene usuarios asociados y no se puede eliminar.";
                    return RedirectToAction(nameof(Index));
                }


                var rolesPermisosExistentes = _context.RolesPermisos.Where(rp => rp.IdRol == id);
                _context.RolesPermisos.RemoveRange(rolesPermisosExistentes);


                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Rol eliminado correctamente.";
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

        private bool RoleExists(int id)
        {
            return _context.Roles.Any(e => e.IdRol == id);
        }

        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }
    }
}