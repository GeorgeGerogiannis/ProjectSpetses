namespace ProjectSpetses.Models.Entities
{
    public class Blank_item
    {//this is near identical with Quiz Item, when DB is developed we should consider merging these
        public required ushort Id { get; set; }
        public required string Difficulty { get; set; } //values: easy, hard
        public required string Description1 { get; set; }
        public required string Description2 { get; set; }
        public required string Solution { get; set; }
        public required List<string> Answers { get; set; }
        public required ushort SectionId { get; set; }

        public Section Section { get; set; }
    }
}
