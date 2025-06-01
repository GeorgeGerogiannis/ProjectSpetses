using System.ComponentModel.DataAnnotations;

namespace ProjectSpetses.Models.Entities
{
    public class User
    {
        [Key]
        public required Guid Id { get; set; }
        [MinLength(2), MaxLength(30)]
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Salt { get; set; }

        public Stats Stats { get; set; }
    }
}
