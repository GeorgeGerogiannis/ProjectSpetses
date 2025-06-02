using ProjectSpetses.Models.Entities;

namespace ProjectSpetses.Models
{
    public class CategoryViewModel
    {
        public required ushort SectionId { get; set; }
        public required ushort CategoryId { get; set; }
        public required string CategoryName { get; set; }
        public Content? Content { get; set; }
        public required ushort PageCount { get; set; }
    }
}
