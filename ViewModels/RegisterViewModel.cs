using System.ComponentModel.DataAnnotations;

namespace EvesysWebApp.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El Nombre Completo es obligatorio.")]
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El Email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de Email inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La Contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("Contrasena", ErrorMessage = "La contraseña y la confirmación no coinciden.")]
        public string ConfirmarContrasena { get; set; }
    }
}