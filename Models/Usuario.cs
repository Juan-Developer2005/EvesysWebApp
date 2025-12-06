using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EvesysWebApp.Models
{
    public class Usuario
    {
        [Key]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreCompleto { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string Contrasena { get; set; } // Agregamos Contraseña

        [StringLength(50)]
        public string? Telefono { get; set; } // Hacemos el teléfono opcional

        // Clave Foránea a Rol
        public int RolId { get; set; }
        [ForeignKey("RolId")]
        public Rol? Rol { get; set; } // Propiedad de navegación

        // Colección de eventos creados por este usuario
        public ICollection<Evento>? EventosCreados { get; set; }
    }
}