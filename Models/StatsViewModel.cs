namespace ProjectSpetses.Models
{
    public class StatsViewModel
    {
        public string Username { get; set; }
        public required uint TotalPoints { get; set; }
        public required ushort SectionsCompleted { get; set; }
        public required ushort CategoriesCompleted { get; set; }
        public required DateTime CreatedAt { get; set; }
    }
}
