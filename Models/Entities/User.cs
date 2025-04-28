namespace ProjectSpetses.Models.Entities
{
    public class User
    {
        public required Guid Id { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Salt { get; set; }

        public Stats Stats { get; set; }
    }
}
