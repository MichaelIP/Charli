using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace McpNetwork.Charli.Server.Models.Entities
{
    [Table("Devices", Schema = "dbo")]
    public partial class Device
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        [Column("DeviceID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DeviceId { get; set; }

        /// <summary>
        /// User #
        /// </summary>
        [Column("UserID")]
        public int UserId { get; set; }

        /// <summary>
        /// Platform
        /// </summary>
        [Required]
        [StringLength(512)]
        public string Platform { get; set; }

        /// <summary>
        /// Os Version
        /// </summary>
        [Required]
        [StringLength(512)]
        public string OsVersion { get; set; }

        /// <summary>
        /// Device key
        /// </summary>
        [Required]
        [StringLength(512)]
        public string DeviceKey { get; set; }

        /// <summary>
        /// Notification token
        /// </summary>
        [StringLength(512)]
        public string NotificationToken { get; set; }

        /// <summary>
        /// Active
        /// </summary>
        public bool Active { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdateDate { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("Device")]
        public virtual User User { get; set; }
    }
}
