namespace FYPWebAPI.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("MapPointToSection")]
    public class MapPointToSection
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        public Section Section { get; set; }

        [Required]
        public MapPoint MapPoint { get; set; }



    }
}