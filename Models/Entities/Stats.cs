namespace ProjectSpetses.Models.Entities
{
    public class Stats
    {
        public required Guid Id { get; set; }
        public required Guid UserId { get; set; }

        public User User { get; set; }
    }
}
