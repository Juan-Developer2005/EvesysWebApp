using EvesysWebApp.Data;
using EvesysWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization; // Para usar el atributo [AllowAnonymous]

namespace EvesysWebApp.Controllers
{
    // El Login y Logout deben permitir el acceso a cualquiera
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Auth/Login (Muestra el formulario)
        public IActionResult Login()
        {
            return View();
        }

        // POST: Auth/Login (Procesa el inicio de sesión)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Buscar usuario, incluyendo su Rol asociado
                var user = await _context.Usuarios
                    .Include(u => u.Rol)
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.Contrasena == model.Contrasena);

                if (user != null)
                {
                    // Crear Claims (Identidad del usuario)
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UsuarioId.ToString()),
                        new Claim(ClaimTypes.Name, user.NombreCompleto),
                        new Claim(ClaimTypes.Email, user.Email),
                        // ¡IMPORTANTE! Asigna el Nombre del Rol (Ej: Administrador, Gerente)
                        new Claim(ClaimTypes.Role, user.Rol.Nombre)
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Crear la Cookie de Autenticación y logear al usuario
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    // Redirigir al usuario a la página a la que quería acceder o al Inicio
                    return Redirect(Request.Query["ReturnUrl"].FirstOrDefault() ?? "/");
                }

                ModelState.AddModelError(string.Empty, "Credenciales no válidas. Revise su email y contraseña.");
            }
            return View(model);
        }

        // POST: Auth/Logout (Cierra la sesión)
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}