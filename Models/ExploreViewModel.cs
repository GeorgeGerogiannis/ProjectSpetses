using ProjectSpetses.Models.Entities;

namespace ProjectSpetses.Models
{
    public class ExploreViewModel
    {
        public required List<Section> Sections { get; set; }
        public required List<ushort> CategoryCount { get; set; }
        public required List<ushort> CompletedCategories { get; set; }
        public required uint Points { get; set; }
    }
}
