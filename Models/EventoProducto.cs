using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EvesysWebApp.Models
{
    public class EventoProducto
    {
        // Clave Compuesta: Se define en OnModelCreating del DbContext
        public int EventoId { get; set; }
        public int ProductoId { get; set; }

        public int Cantidad { get; set; }

        // Propiedades de navegación (Para acceder a la información completa)
        [ForeignKey("EventoId")]
        public Evento? Evento { get; set; }

        [ForeignKey("ProductoId")]
        public Producto? Producto { get; set; }
    }
}