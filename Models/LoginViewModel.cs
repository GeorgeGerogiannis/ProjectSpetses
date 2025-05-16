using System.ComponentModel.DataAnnotations;

namespace ProjectSpetses.Models
{
    public class LoginViewModel
    {
        [Required]
        [MinLength(2, ErrorMessage = "Username must be at least 2 characters"), MaxLength(30, ErrorMessage = "Username must be at most 30 characters")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
