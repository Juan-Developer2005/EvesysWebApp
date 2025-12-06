using EvesysWebApp.Data;
using EvesysWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; // Necesario para [Authorize]

namespace EvesysWebApp.Controllers
{
    // Solo Administradores pueden gestionar roles
    [Authorize(Roles = "Administrador")]
    public class RolesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===================================
        // READ (Index)
        // ===================================
        // GET: Roles
        public async Task<IActionResult> Index()
        {
            return View(await _context.Roles.ToListAsync());
        }

        // ===================================
        // CREATE
        // ===================================
        // GET: Roles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre")] Rol rol)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rol);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(rol);
        }

        // ===================================
        // EDIT (Update)
        // ===================================
        // GET: Roles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rol = await _context.Roles.FindAsync(id);
            if (rol == null)
            {
                return NotFound();
            }
            return View(rol);
        }

        // POST: Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RolId,Nombre")] Rol rol)
        {
            if (id != rol.RolId)
            {
                return NotFound();
            }

            // Nota: Aquí se omiten validaciones complejas de concurrencia
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rol);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Roles.Any(e => e.RolId == rol.RolId))
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
            return View(rol);
        }

        // ===================================
        // DELETE
        // ===================================
        // GET: Roles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rol = await _context.Roles
                .FirstOrDefaultAsync(m => m.RolId == id);

            if (rol == null)
            {
                return NotFound();
            }

            // Advertencia: Evitar eliminar roles que tienen usuarios asociados
            var usuariosAsociados = await _context.Usuarios.CountAsync(u => u.RolId == id);
            if (usuariosAsociados > 0)
            {
                // Agregamos un mensaje de advertencia si hay usuarios
                ViewBag.DeletionWarning = $"ADVERTENCIA: Hay {usuariosAsociados} usuarios asignados a este rol. No podrá eliminarse si tiene referencias activas.";
            }

            return View(rol);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol != null)
            {
                try
                {
                    _context.Roles.Remove(rol);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    // Manejar error si el rol todavía está en uso (llave foránea)
                    ModelState.AddModelError(string.Empty, "No se puede eliminar este rol porque tiene usuarios asociados. Reasigne los usuarios antes de intentar eliminar.");
                    return View("Delete", rol);
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}