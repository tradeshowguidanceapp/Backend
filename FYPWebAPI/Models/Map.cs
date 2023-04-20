namespace FYPWebAPI.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Map")]
    public class Map
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        public string MapName { get; set; }

        public string ContentType { get; set; }
        public byte[]? MapImage { get; set; }

        public int sizingLocation1X { get; set; }

        public int sizingLocation1Y { get; set; }

        public int sizingLocation2X { get; set; }

        public int sizingLocation2Y { get; set; }

        public double metresBetweenPoints { get; set; }

    }
}
