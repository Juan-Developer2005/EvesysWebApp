using EvesysWebApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EvesysWebApp.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<ApplicationDbContext>>()))
            {
                // 1. Crear Roles si no existen
                if (!await context.Roles.AnyAsync())
                {
                    context.Roles.AddRange(
                        new Rol { Nombre = "Administrador" },
                        new Rol { Nombre = "Gerente de Eventos" },
                        new Rol { Nombre = "Técnico de Logística" }
                    );
                    await context.SaveChangesAsync();
                }

                // 2. Crear Usuario Administrador si no existe
                var administradorRol = await context.Roles.FirstOrDefaultAsync(r => r.Nombre == "Administrador");

                // Solo si el rol Administrador existe y NO hay un usuario con el email de administrador
                if (administradorRol != null && !await context.Usuarios.AnyAsync(u => u.Email == "admin@evesys.com"))
                {
                    var administrador = new Usuario
                    {
                        NombreCompleto = "Admin Principal",
                        Email = "admin@evesys.com",
                        Contrasena = "123456", 
                        RolId = administradorRol.RolId
                    };
                    context.Usuarios.Add(administrador);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}