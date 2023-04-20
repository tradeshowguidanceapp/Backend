namespace FYPWebAPI.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ProjectLocation")]
    public class ProjectLocation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        public Project Project { get; set; }

        public int? X { get; set; }
        public int? Y { get; set; }

        [Required]
        public Section Section { get; set; }
    }
}
