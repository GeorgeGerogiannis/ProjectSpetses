namespace ProjectSpetses.Models.Entities
{
    public class Match_item
    {
        public required ushort Id { get; set; }
        public required string Difficulty { get; set; } //values: easy, hard
        public required string Image { get; set; }
        public required string Solution { get; set; }
        public required ushort SectionId { get; set; }

        public Section Section { get; set; }
    }
}
