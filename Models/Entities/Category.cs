namespace ProjectSpetses.Models.Entities
{
    public class Category
    {
        public required ushort Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public required ushort SectionId { get; set; }

        public Section Section { get; set; }
    }
}
