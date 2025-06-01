using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectSpetses.Models.Entities
{
    public class Section
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public required ushort Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }

        [DataType(DataType.ImageUrl)]
        public string? ImageUrl { get; set; }
    }
}
