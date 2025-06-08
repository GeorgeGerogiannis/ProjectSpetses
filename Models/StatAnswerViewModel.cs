namespace ProjectSpetses.Models
{
    public class StatAnswerViewModel
    {
        public required ushort SectionId { get; set; } //format: "{sectionId}"
        public required string GameType { get; set; } //format: "{gameType}"
        public required ushort GameId { get; set; } //format: "{gameId}"
    }
}
