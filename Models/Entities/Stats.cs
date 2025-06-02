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
        public required List<int> NotificationsGiven { get; set; }

        public User User { get; set; }
    }
}
