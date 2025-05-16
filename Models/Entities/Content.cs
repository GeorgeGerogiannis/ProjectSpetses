using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectSpetses.Models.Entities
{
    public class Content
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public required ushort Id { get; set; }
        public required string Text { get; set; }
        public required ushort CategoryId { get; set; }
        public required ushort Page { get; set; }

        public Category Category { get; set; }
    }
}
