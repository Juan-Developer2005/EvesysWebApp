using System.ComponentModel.DataAnnotations;

namespace EvesysWebApp.Models
{
    public class Producto
    {
        [Key]
        public int ProductoId { get; set; }

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; }

        public decimal Precio { get; set; }

        [StringLength(500)]
        public string? Descripcion { get; set; }

        // Propiedad de navegación Many-to-Many
        public ICollection<EventoProducto>? EventoProductos { get; set; }
    }
}