namespace ProjectSpetses.Models.Entities
{
    public class Match_item
    {
        public required ushort Id { get; set; }
        public required string Image { get; set; }
        public required string Solution { get; set; }
        public required ushort ContentId { get; set; }

        public Content Content { get; set; }
    }
}
