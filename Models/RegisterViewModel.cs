using System.ComponentModel.DataAnnotations;

namespace ProjectSpetses.Models
{
    public class RegisterViewModel : LoginViewModel
    {
        [Required(ErrorMessage = "Παρακαλώ συμπληρώστε τον κωδικό πρόσβασης")]
        [Compare("Password", ErrorMessage = "Ο κωδικός πρόσβασης δεν ταιριάζει!")]
        public required string ConfirmPassword { get; set; }
    }
}
