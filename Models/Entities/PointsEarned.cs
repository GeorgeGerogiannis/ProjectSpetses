using System.ComponentModel.DataAnnotations;

namespace ProjectSpetses.Models.Entities
{
    public class PointsEarned
    {
        [Key]
        public required Guid Id { get; set; }
        public uint Section1Easy { get; set; }
        public uint Section1Hard { get; set; }
        public uint Section2Easy { get; set; }
        public uint Section2Hard { get; set; }
        public uint Section3Easy { get; set; }
        public uint Section3Hard { get; set; }
        public uint Section4Easy { get; set; }
        public uint Section4Hard { get; set; }

        public User User { get; set; }
    }
}
