using System.ComponentModel.DataAnnotations;

namespace ProjectSpetses.Models.Entities
{
    public class Stats
    {
        [Key]
        public required Guid Id { get; set; }
        public string? LastRead { get; set; }
        public required uint TotalPoints { get; set; }
        public required List<string> CategoriesRead { get; set; }
        public required List<ushort> NotificationsGiven { get; set; }
        public required DateTime CreatedAt { get; set; }

        public User User { get; set; }
    }
}
