namespace ProjectSpetses.Models
{
    public class GamesViewModel
    {
        public required ushort SectionCount { get; set; }
        public List<string> SectionNames { get; set; }
        public List<ushort> CompletedSectionIds { get; set; }
        public Dictionary<string, uint> SectionPointsEarned { get; set; }
    }
}
