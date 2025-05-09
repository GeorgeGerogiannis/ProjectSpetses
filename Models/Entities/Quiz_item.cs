namespace ProjectSpetses.Models.Entities
{
    public class Quiz_item
    {
        public required Guid Section_Id { get; set; }
        public required string Description { get; set; }
        public required string Solution { get; set; }
        public required List<string> Answers { get; set; }
    }
}
