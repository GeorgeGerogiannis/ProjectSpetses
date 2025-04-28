using ProjectSpetses.Models.Entities;

namespace ProjectSpetses.Models
{
    public class SectionViewModel
    {
        public required ushort SectionId { get; set; }
        public required string SectionName { get; set; }
        public required List<Category> Categories { get; set; }
    }
}
