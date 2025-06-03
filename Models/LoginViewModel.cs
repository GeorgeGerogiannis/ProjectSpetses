using System.ComponentModel.DataAnnotations;

namespace ProjectSpetses.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Παρακαλώ συμπληρώστε το όνομα χρήστη")]
        [MinLength(2, ErrorMessage = "Το όνομα χρήστη πρέπει να περιέχει τουλάχιστον 2 χαρακτήρες!"), MaxLength(30, ErrorMessage = "Το όνομα χρήστη πρέπει να περιέχει το πολύ 30 χαρακτήρες!")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Παρακαλώ συμπληρώστε τον κωδικό πρόσβασης")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}
