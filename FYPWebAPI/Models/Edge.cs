namespace FYPWebAPI.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Edge")]
    public class Edge
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        public Vertex VertexA { get; set; }

        [Required]
        public Vertex VertexB { get; set; }

        public Section Section { get; set; }

    }
}