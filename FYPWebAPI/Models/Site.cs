namespace FYPWebAPI.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Site")]
    public class Site
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        public string SiteName { get; set; }

        public string? SiteWebsite { get; set; }

        public string? SiteDescription { get; set; }

        public Section? StartSection { get; set; }


    }
}