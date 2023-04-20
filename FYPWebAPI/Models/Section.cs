namespace FYPWebAPI.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Section")]
    public class Section
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        public string SectionName { get; set; }
        
        public Map? OuterMap { get; set; }

        [Required]
        public Map InnerMap { get; set; }

        [Required]
        public int topLeftX { get; set; }

        [Required]
        public int topLeftY { get; set; }

        [Required]
        public int bottomRightX { get; set; }

        [Required]
        public int bottomRightY { get; set; }

    }
}
