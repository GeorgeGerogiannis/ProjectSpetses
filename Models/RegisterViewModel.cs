using System.ComponentModel.DataAnnotations;

namespace ProjectSpetses.Models
{
    public class RegisterViewModel : LoginViewModel
    {
        [Compare("Password", ErrorMessage = "Password doesn't match, Type again !")]
        public string ConfirmPassword { get; set; }
    }
}
