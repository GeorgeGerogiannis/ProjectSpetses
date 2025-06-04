using System.ComponentModel.DataAnnotations;

namespace ProjectSpetses.Models.Entities
{
    public class Stats
    {
        [Key]
        public required Guid Id { get; set; }
        public string? LastRead { get; set; } //format: "{sectionId}:{categoryId}:{contentPage}"
        public required uint TotalPoints { get; set; }
        public required List<string> CategoriesRead { get; set; } //format: "{sectionId}:{categoryId}"
        public required List<ushort> NotificationsGiven { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required List<string> CorrectAnswers { get; set; } //format: "{sectionId}:{gameType}:{gameId}"
        public required List<string> WrongAnswers { get; set; } //format: "{sectionId}:{gameType}:{gameId}"

        public User User { get; set; }
    }
}
