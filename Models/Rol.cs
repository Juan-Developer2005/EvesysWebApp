using System.ComponentModel.DataAnnotations;

namespace EvesysWebApp.Models
{
    public class Rol
    {
        [Key]
        public int RolId { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

        // Propiedad de navegación: Colección de usuarios asociados a este rol
        public ICollection<Usuario>? Usuarios { get; set; }
    }
}