using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyect.Models;

namespace Proyect.Controllers
{
    [Authorize]
    public class RolesPermisosController : Controller
    {
        private readonly ProyectContext _context;

        public RolesPermisosController(ProyectContext context)
        {
            _context = context;
        }

        // GET: RolesPermisos
        //[Authorize(Policy = "ViewRolesPermisos")] // Aplicamos la política de permisos
        public async Task<IActionResult> Index()
        {
            var proyectContext = _context.RolesPermisos
                .Include(r => r.IdPermisoNavigation)
                .Include(r => r.IdRolNavigation);
            return View(await proyectContext.ToListAsync());
        }

        // GET: RolesPermisos/Create

        //[Authorize(Policy = "ManageRolesPermisos")]
        public IActionResult Create()
        {
            ViewData["IdPermiso"] = new SelectList(_context.Permisos, "IdPermiso", "NombrePermiso");
            ViewData["IdRol"] = new SelectList(_context.Roles, "IdRol", "NombreRol");

            ViewData["Permisos"] = _context.Permisos.ToList();
            return View();
        }

        // POST: RolesPermisos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Policy = "ManageRolesPermisos")]
        public async Task<IActionResult> Create(int IdRol, List<int> selectedPermisos)
        {
            if (ModelState.IsValid)
            {
                if (selectedPermisos == null || !selectedPermisos.Any())
                {
                    ModelState.AddModelError("selectedPermisos", "Debe seleccionar al menos un permiso.");
                    ViewData["Permisos"] = _context.Permisos.ToList();
                    ViewData["IdRol"] = new SelectList(_context.Roles, "IdRol", "NombreRol", IdRol);
                    return View();
                }

                // Validar que el rol existe
                var rol = await _context.Roles.FindAsync(IdRol);
                if (rol == null)
                {
                    ModelState.AddModelError("IdRol", "El rol seleccionado no existe.");
                    return View();
                }

                // Validar que los permisos existen
                var permisosValidos = _context.Permisos.Where(p => selectedPermisos.Contains(p.IdPermiso)).ToList();
                if (permisosValidos.Count != selectedPermisos.Count)
                {
                    ModelState.AddModelError("selectedPermisos", "Uno o más permisos seleccionados no son válidos.");
                    return View();
                }

                foreach (var idPermiso in selectedPermisos)
                {
                    var rolesPermiso = new RolesPermiso
                    {
                        IdRol = IdRol,
                        IdPermiso = idPermiso
                    };
                    _context.Add(rolesPermiso);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }


            ViewData["IdPermiso"] = new SelectList(_context.Permisos, "IdPermiso", "NombrePermiso");
            ViewData["IdRol"] = new SelectList(_context.Roles, "IdRol", "NombreRol");
            ViewData["Permisos"] = _context.Permisos.ToList();
            return View();
        }

        // GET: RolesPermisos/Edit/5
        //[Authorize(Policy = "ManageRolesPermisos")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rolesPermiso = await _context.RolesPermisos.FindAsync(id);
            if (rolesPermiso == null)
            {
                return NotFound();
            }

            ViewData["IdPermiso"] = new SelectList(_context.Permisos, "IdPermiso", "NombrePermiso", rolesPermiso.IdPermiso);
            ViewData["IdRol"] = new SelectList(_context.Roles, "IdRol", "NombreRol", rolesPermiso.IdRol);
            return View(rolesPermiso);
        }

        // POST: RolesPermisos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Policy = "ManageRolesPermisos")]
        public async Task<IActionResult> Edit(int id, [Bind("IdRolPermiso,IdRol,IdPermiso")] RolesPermiso rolesPermiso)
        {
            if (id != rolesPermiso.IdRolPermiso)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rolesPermiso);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RolesPermisoExists(rolesPermiso.IdRolPermiso))
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

            ViewData["IdPermiso"] = new SelectList(_context.Permisos, "IdPermiso", "NombrePermiso", rolesPermiso.IdPermiso);
            ViewData["IdRol"] = new SelectList(_context.Roles, "IdRol", "NombreRol", rolesPermiso.IdRol);
            return View(rolesPermiso);

        }
        private bool RolesPermisoExists(int id)
        {
            return _context.RolesPermisos.Any(e => e.IdRolPermiso == id);
        }

        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }
    }
}
