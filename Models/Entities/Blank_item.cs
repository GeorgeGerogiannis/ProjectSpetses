namespace ProjectSpetses.Models.Entities
{
    public class Blank_item
    {//this is near identical with Quiz Item, when DB is developed we should consider merging these
        public required Guid Section_Id { get; set; }
        public required string Description1 { get; set; }
        public required string Description2 { get; set; }
        public required string Solution { get; set; }
        public required List<string> Answers { get; set; }
    }
}
