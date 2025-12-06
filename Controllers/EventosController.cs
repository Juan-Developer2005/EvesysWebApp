using EvesysWebApp.Data;
using EvesysWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization; // ¡AÑADE ESTE USING!

namespace EvesysWebApp.Controllers
{
    [Authorize] // Cualquier usuario autenticado puede acceder a los eventos
    public class EventosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =================================================================
        // CRUD BÁSICO DE EVENTOS
        // =================================================================

        // GET: Eventos
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Eventos.Include(e => e.Usuario);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Eventos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos
                .Include(e => e.Usuario)
                .FirstOrDefaultAsync(m => m.EventoId == id);

            if (evento == null)
            {
                return NotFound();
            }

            return View(evento);
        }

        // GET: Eventos/Create
        public IActionResult Create()
        {
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "NombreCompleto");
            return View();
        }

        // POST: Eventos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventoId,Titulo,FechaHora,Descripcion,UsuarioId")] Evento evento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(evento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "NombreCompleto", evento.UsuarioId);
            return View(evento);
        }

        // GET: Eventos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null)
            {
                return NotFound();
            }

            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "NombreCompleto", evento.UsuarioId);
            return View(evento);
        }

        // POST: Eventos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventoId,Titulo,FechaHora,Descripcion,UsuarioId")] Evento evento)
        {
            if (id != evento.EventoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(evento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Eventos.Any(e => e.EventoId == evento.EventoId))
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
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "NombreCompleto", evento.UsuarioId);
            return View(evento);
        }

        // GET: Eventos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos
                .Include(e => e.Usuario)
                .FirstOrDefaultAsync(m => m.EventoId == id);

            if (evento == null)
            {
                return NotFound();
            }

            return View(evento);
        }

        // POST: Eventos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var evento = await _context.Eventos.FindAsync(id);
            if (evento != null)
            {
                _context.Eventos.Remove(evento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // =================================================================
        // FUNCIONALIDAD DE ASIGNACIÓN Y ELIMINACIÓN DE PRODUCTOS (M:M)
        // =================================================================

        // GET: Eventos/AsignarProductos/5
        public async Task<IActionResult> AsignarProductos(int id)
        {
            var evento = await _context.Eventos
                .Include(e => e.EventosProductos) // Incluye la tabla de unión
                .ThenInclude(ep => ep.Producto)    // Luego incluye los Productos relacionados
                .FirstOrDefaultAsync(m => m.EventoId == id);

            if (evento == null)
            {
                return NotFound();
            }

            // Obtener todos los productos disponibles para el SelectList
            ViewData["ProductosDisponibles"] = new SelectList(_context.Productos, "ProductoId", "Nombre");

            return View(evento);
        }

        // POST: Eventos/AsignarProductos/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarProductos(int EventoId, int ProductoId, int Cantidad)
        {
            if (Cantidad <= 0)
            {
                TempData["ErrorMessage"] = "La cantidad debe ser mayor a cero.";
                return RedirectToAction(nameof(AsignarProductos), new { id = EventoId });
            }

            var eventoProducto = new EventoProducto
            {
                EventoId = EventoId,
                ProductoId = ProductoId,
                Cantidad = Cantidad
            };

            // Verificar si la relación ya existe (para evitar duplicados)
            if (_context.EventosProductos.Any(ep => ep.EventoId == EventoId && ep.ProductoId == ProductoId))
            {
                TempData["ErrorMessage"] = "Este producto ya está asignado a este evento. Usa la edición para cambiar la cantidad.";
            }
            else
            {
                _context.Add(eventoProducto);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Producto asignado correctamente.";
            }

            return RedirectToAction(nameof(AsignarProductos), new { id = EventoId });
        }

        // POST: Eventos/EditarCantidadAsignada
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarCantidadAsignada(int EventoId, int ProductoId, int NuevaCantidad)
        {
            if (NuevaCantidad <= 0)
            {
                TempData["ErrorMessage"] = "La cantidad debe ser mayor a cero para poder editarla.";
                return RedirectToAction(nameof(AsignarProductos), new { id = EventoId });
            }

            var eventoProducto = await _context.EventosProductos
                .FirstOrDefaultAsync(ep => ep.EventoId == EventoId && ep.ProductoId == ProductoId);

            if (eventoProducto != null)
            {
                eventoProducto.Cantidad = NuevaCantidad;
                _context.Update(eventoProducto);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cantidad actualizada correctamente.";
            }
            else
            {
                TempData["ErrorMessage"] = "Asignación no encontrada.";
            }

            return RedirectToAction(nameof(AsignarProductos), new { id = EventoId });
        }

        // POST: Eventos/EliminarProductoAsignado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarProductoAsignado(int EventoId, int ProductoId)
        {
            var eventoProducto = await _context.EventosProductos
                .FirstOrDefaultAsync(ep => ep.EventoId == EventoId && ep.ProductoId == ProductoId);

            if (eventoProducto != null)
            {
                _context.EventosProductos.Remove(eventoProducto);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Producto eliminado de la asignación correctamente.";
            }
            else
            {
                TempData["ErrorMessage"] = "Asignación no encontrada.";
            }

            return RedirectToAction(nameof(AsignarProductos), new { id = EventoId });
        }
    }
}