using System.ComponentModel.DataAnnotations;

namespace ProjectSpetses.Models.Entities
{
    public class User
    {
        [Key]
        public required Guid Id { get; set; }
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters"), MaxLength(30, ErrorMessage = "Username must be at most 30 characters")]
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Salt { get; set; }

        public Stats Stats { get; set; }
    }
}
