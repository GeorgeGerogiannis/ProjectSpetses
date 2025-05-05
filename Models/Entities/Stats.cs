namespace ProjectSpetses.Models.Entities
{
    public class Stats
    {
        public required Guid Id { get; set; }
        public string? LastRead { get; set; }

        public User User { get; set; }
    }
}
