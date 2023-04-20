namespace FYPWebAPI.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("MapPoint")]
    public class MapPoint
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string Name { get; set; }

        [Required]
        public NetworkDevice NetworkDevice { get; set; }

        public int? Range { get; set; }

        public int? X { get; set; }
        public int? Y { get; set; }

        [Required]
        public Map Map { get; set; }
    }
}
