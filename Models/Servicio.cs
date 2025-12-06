using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EvesysWebApp.Models
{
    public class Servicio
    {
        [Key]
        public int ServicioId { get; set; }

        [Required(ErrorMessage = "El nombre del servicio es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(500)]
        public string Descripcion { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Costo por Hora/Día")]
        public decimal Costo { get; set; }

        [Required(ErrorMessage = "La unidad de medida es obligatoria.")]
        [StringLength(50)]
        [Display(Name = "Unidad de Medida")]
        // Ejemplo: "Hora", "Día", "Evento", "Personal"
        public string UnidadMedida { get; set; }
    }
}