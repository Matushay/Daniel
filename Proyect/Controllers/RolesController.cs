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
        public IActionResult Create([Bind("IdRol,NombreRol,Descripcion,Activo")] Role role, List<int> selectedPermisos)
        {



            var nombresExistentes = _context.Roles.Select(r => r.NombreRol).ToList();
            var validator = new RolValidator(nombresExistentes);
            var validationResult = validator.Validate(role);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.ErrorMessage);
                }


                ViewData["Permisos"] = _context.Permisos.ToList();
                return View(role);
            }


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

        // Método para actualizar el estado de un rol (activo/inactivo)
        [HttpPost]
        public async Task<IActionResult> ActualizarEstado(int id, bool estado)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol != null)
            {
                rol.Activo = estado; // Actualizamos el estado (activo o inactivo)
                _context.Update(rol);
                await _context.SaveChangesAsync();
                return Ok(); // Retornamos un 200 OK si la actualización fue exitosa
            }
            return BadRequest(); // Si no encontramos el rol, retornamos un error
        }

        // GET: Roles/Edit/5
        //[Authorize(Policy = "ManageRoles")]
        public async Task<IActionResult> Edit(int? id)
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

            //pasar 

            ViewData["Permisos"] = await _context.Permisos.ToListAsync();


            ViewData["SelectedPermisos"] = role.RolesPermisos.Select(rp => rp.IdPermiso).ToList();

            return View(role);
        }


        // POST: Roles/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdRol,NombreRol,Descripcion,Activo")] Role role, List<int> selectedPermisos)
        {
            if (id != role.IdRol)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    _context.Update(role);
                    await _context.SaveChangesAsync();


                    var permisosExistentes = _context.RolesPermisos.Where(rp => rp.IdRol == id).ToList();
                    _context.RolesPermisos.RemoveRange(permisosExistentes);


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


        private bool RoleExists(int id)
        {
            return _context.Roles.Any(e => e.IdRol == id);
        }
    }

}