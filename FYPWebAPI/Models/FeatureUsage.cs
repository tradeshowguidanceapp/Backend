using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FYPWebAPI.Models
{
    public class FeatureUsage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string macHash { get; set; }

        public string featureName { get; set; }

        public int usageCountInSeconds { get; set; }
    }
}
