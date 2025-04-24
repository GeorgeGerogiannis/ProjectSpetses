namespace ProjectSpetses.Models.Entities
{
    public class Section
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
