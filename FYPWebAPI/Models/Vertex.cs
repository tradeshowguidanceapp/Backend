using System.ComponentModel.DataAnnotations.Schema;

namespace FYPWebAPI.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Vertex")]
    public class Vertex
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        public string VertexName { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public Section Section { get; set; }

    }
}
