namespace FYPWebAPI.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("NetworkDevice")]
    public class NetworkDevice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        public string DeviceName { get; set; }

        [Required]
        public string DeviceMAC { get; set; }

        public string? DeviceUUIDs { get; set; }

        public bool? IsPublic { get; set; }

        public bool? IsBluetooth { get; set; }
    }
}
