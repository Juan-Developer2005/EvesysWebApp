using Microsoft.EntityFrameworkCore;
using EvesysWebApp.Models; // Importar los modelos

namespace EvesysWebApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // --- Registro de Tablas (DbSet) ---
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<EventoProducto> EventosProductos { get; set; }

        public DbSet<Servicio> Servicios { get; set; }

        // --- Configuración del Modelo y Data Seeding ---
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. INSERCIÓN DE DATOS INICIALES (SEEDING)
            // Se insertan los roles para que las claves foráneas de Usuario sean válidas (1, 2, 3)
            modelBuilder.Entity<Rol>().HasData(
                new Rol { RolId = 1, Nombre = "Administrador" }, // ID 1
                new Rol { RolId = 2, Nombre = "Empleado" },      // ID 2
                new Rol { RolId = 3, Nombre = "Cliente" }        // ID 3
            );

            // 2. DEFINICIÓN DE CLAVE COMPUESTA
            // Se define la clave compuesta para la tabla de unión Muchos a Muchos
            modelBuilder.Entity<EventoProducto>()
                .HasKey(ep => new { ep.EventoId, ep.ProductoId });
        }
    }
}