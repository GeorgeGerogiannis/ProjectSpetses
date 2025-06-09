using System.ComponentModel.DataAnnotations;

namespace ProjectSpetses.Models.Entities
{
    public class Stats
    {
        [Key]
        public required Guid Id { get; set; }
        public string? LastRead { get; set; } //format: "{sectionId}:{categoryId}:{contentPage}"
        public required uint TotalPoints { get; set; }
        public required List<string> CategoriesCompleted { get; set; } //format: "{sectionId}:{categoryId}"
        public required List<ushort> SectionsCompleted { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required List<StatAnswerViewModel> CorrectAnswers { get; set; } //serialised StatsAnswersViewModel
        public required List<StatAnswerViewModel> WrongAnswers { get; set; } //serialised StatsAnswersViewModel

        public User User { get; set; }
    }
}
