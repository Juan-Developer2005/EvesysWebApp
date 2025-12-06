using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EvesysWebApp.Models
{
    public class Evento
    {
        [Key]
        public int EventoId { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(200)]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "La fecha y hora son obligatorias.")]
        [DataType(DataType.DateTime)]
        public DateTime FechaHora { get; set; }

        [StringLength(500)]
        public string? Descripcion { get; set; }

        // --- Clave Foránea (1:M con Usuario) ---
        [Required(ErrorMessage = "El usuario creador es obligatorio.")]
        public int UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        // --- Propiedad de navegación Many-to-Many (Relación con EventoProducto) ---
        // ESTA ES LA PROPIEDAD QUE FALTABA Y SOLUCIONA EL ERROR CS1061
        public ICollection<EventoProducto>? EventosProductos { get; set; }
    }
}