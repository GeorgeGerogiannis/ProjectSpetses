using ProjectSpetses.Models.Entities;

namespace ProjectSpetses.Models
{
    public class ExploreViewModel
    {
        public required List<Section> Sections { get; set; }
        public required List<bool> Completed { get; set; }
        public required uint Points { get; set; }
    }
}
