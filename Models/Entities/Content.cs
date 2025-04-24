namespace ProjectSpetses.Models.Entities
{
    public class Content
    {
        public required Guid Id { get; set; }
        public required Guid SectionId { get; set; }

        public Section Section { get; set; }
    }
}
