namespace FYPWebAPI.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Project")]
    public class Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        public string ProjectID { get; set; }

        public string? ProjectName { get; set; }

        public string? ProjectDescription { get; set; }

        public string? ContentType { get; set; }
        public byte[]? ProjectImage { get; set; }

        public string? ProjectOwner { get; set; }

        public string? ContactEmail { get; set; }

        public string? Supervisor { get; set; }

        public string? Tags { get; set; }

        public string? ProjectWebsite { get; set; }


    }
}
