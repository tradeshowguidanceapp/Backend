namespace FYPWebAPI.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Hint")]
    public class Hint
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string Name { get; set; }

        public int? X { get; set; }
        public int? Y { get; set; }

        [Required]
        public Map Map { get; set; }
        
        [Required]
        public string HintText { get; set; }

        public byte[]? HintImage { get; set; }

        public string? ContentType { get; set; }
    }
}