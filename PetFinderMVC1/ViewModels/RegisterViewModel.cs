using System.ComponentModel.DataAnnotations;

namespace PetFinderMVC1.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string Telefono { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }
    }
}
