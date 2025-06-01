using System.ComponentModel.DataAnnotations;

namespace ProjectSpetses.Models.Entities
{
    public class Stats
    {
        [Key]
        public required Guid Id { get; set; }
        public string? LastRead { get; set; }

        public User User { get; set; }
    }
}
