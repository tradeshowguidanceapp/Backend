namespace FYPWebAPI.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Door")]
    public class Door
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        public Vertex Vertex { get; set; }

        public Section Section { get; set; }

        // 0 = not a door, 1 = entrance, 2 = exit, 3 = both
        public int DoorType { get; set; }

    }
}