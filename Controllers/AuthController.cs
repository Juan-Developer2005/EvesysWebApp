using EvesysWebApp.Data;
using EvesysWebApp.Models; // Necesario para la clase Usuario
using EvesysWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization; // Para usar el atributo [AllowAnonymous]

namespace EvesysWebApp.Controllers
{
    // El Login, Registro y Logout deben permitir el acceso a cualquiera
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===================================
        // LOGIN
        // ===================================

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
                        // ¡IMPORTANTE! Asigna el Nombre del Rol
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

        // ===================================
        // REGISTRO
        // ===================================

        // GET: Auth/Register (Muestra el formulario de registro)
        public IActionResult Register()
        {
            return View();
        }

        // POST: Auth/Register (Procesa el registro del usuario)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Verificar si el email ya existe
                if (await _context.Usuarios.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Este Email ya se encuentra registrado.");
                    return View(model);
                }

                // 2. Asignar Rol por Defecto (buscamos "Técnico de Logística")
                var defaultRol = await _context.Roles.FirstOrDefaultAsync(r => r.RolId == 3);

                if (defaultRol == null)
                {
                    ModelState.AddModelError(string.Empty, "Error de sistema: El Rol por defecto (ID 3) no existe en la base de datos. Ejecute las migraciones y SeedData.");
                    return View(model);
                }

                // 3. Crear nuevo usuario
                var newUser = new Usuario
                {
                    NombreCompleto = model.NombreCompleto,
                    Email = model.Email,
                    Contrasena = model.Contrasena, // Nota: En un sistema real se debería hashear la contraseña
                    RolId = defaultRol.RolId
                };

                _context.Usuarios.Add(newUser);
                await _context.SaveChangesAsync();

                // Opcional: Loguear al usuario automáticamente después del registro
                // Llama al método Login con los datos del nuevo usuario si lo deseas.
                // Por ahora, solo redirigimos al login para que inicie sesión.

                TempData["SuccessMessage"] = "¡Registro exitoso! Por favor inicia sesión.";
                return RedirectToAction(nameof(Login));
            }

            return View(model);
        }


        // ===================================
        // LOGOUT
        // ===================================

        // POST: Auth/Logout (Cierra la sesión)
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}