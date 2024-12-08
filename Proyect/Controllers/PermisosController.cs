
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using Proyect.Models;

namespace Proyect.Controllers
{
    [Authorize]
    public class PermisosController : Controller
    {
        private readonly ProyectContext _context;

        public PermisosController(ProyectContext context)
        {
            _context = context;
        }

        // GET: Permisos
        //[Authorize(Policy = "ViewPermisos")] // Requiere el permiso de visualizar permisos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Permisos.ToListAsync());
        }

        // GET: Permisos/Details/5
        //[Authorize(Policy = "ViewPermisos")] // Requiere el permiso de visualizar detalles
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permiso = await _context.Permisos.FirstOrDefaultAsync(m => m.IdPermiso == id);
            if (permiso == null)
            {
                return NotFound();
            }

            return View(permiso);
        }

        // GET: Permisos/Create
        //[Authorize(Policy = "CreatePermisos")] // Requiere el permiso de crear
        public IActionResult Create()
        {
            return View();
        }

        // POST: Permisos/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Policy = "CreatePermisos")] // Requiere el permiso de crear
        public async Task<IActionResult> Create([Bind("IdPermiso,NombrePermiso,Descripcion")] Permiso permiso)
        {
            if (ModelState.IsValid)
            {
                // Validación adicional: evitar duplicados en el controlador si falla FluentValidation
                if (_context.Permisos.Any(p => p.NombrePermiso == permiso.NombrePermiso))
                {
                    ModelState.AddModelError("NombrePermiso", "Ya existe un permiso con este nombre.");
                    return View(permiso);
                }

                _context.Add(permiso);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(permiso);
        }

        // GET: Permisos/Edit/5
        //[Authorize(Policy = "EditPermisos")] // Requiere el permiso de editar
        // GET: Permisos/Edit/5
        // [Authorize(Policy = "EditPermisos")] // Requiere el permiso de editar
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permiso = await _context.Permisos.FindAsync(id);
            if (permiso == null)
            {
                return NotFound();
            }

            return View(permiso);
        }


        // POST: Permisos/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Authorize(Policy = "EditPermisos")] // Requiere el permiso de editar
        public async Task<IActionResult> Edit(int id, [Bind("IdPermiso,NombrePermiso,Descripcion")] Permiso permiso)
        {
            if (id != permiso.IdPermiso)
            {
                return NotFound();
            }

            // Validación adicional de unicidad
            if (_context.Permisos.Any(p => p.NombrePermiso == permiso.NombrePermiso && p.IdPermiso != id))
            {
                ModelState.AddModelError("NombrePermiso", "Ya existe otro permiso con este nombre.");
                return View(permiso); // Regresa a la vista con el error de validación
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(permiso);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PermisoExists(permiso.IdPermiso))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw; // Relanzar excepción si el problema es distinto
                    }
                }

            }

            return View(permiso); // Regresa a la vista si el modelo no es válido
        }

        // GET: Permisos/Delete/5
        //[Authorize(Policy = "DeletePermisos")] // Requiere el permiso de eliminar
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permiso = await _context.Permisos.FirstOrDefaultAsync(m => m.IdPermiso == id);
            if (permiso == null)
            {
                return NotFound();
            }

            return View(permiso);
        }

        // POST: Permisos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[Authorize(Policy = "DeletePermisos")] // Requiere el permiso de eliminar
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var permiso = await _context.Permisos.FindAsync(id);
            if (permiso != null)
            {
                _context.Permisos.Remove(permiso);
                await _context.SaveChangesAsync();
            }


            return RedirectToAction(nameof(Index));
        }

        private bool PermisoExists(int id)
        {
            return _context.Permisos.Any(e => e.IdPermiso == id);
        }
        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }
    }
}
