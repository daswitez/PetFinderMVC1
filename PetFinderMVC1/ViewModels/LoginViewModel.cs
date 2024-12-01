using System.ComponentModel.DataAnnotations;

namespace PetFinderMVC1.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }
    }
}
