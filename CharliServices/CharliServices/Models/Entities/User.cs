using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace McpNetwork.Charli.Server.Models.Entities
{
    [Table("Users", Schema = "dbo")]
    public partial class User
    {
        public User()
        {
            Device = new HashSet<Device>();
            UserSecurityCode = new HashSet<UserSecurityCode>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        [Required]
        [StringLength(512)]
        public string UserName { get; set; }

        /// <summary>
        /// First name
        /// </summary>
        [StringLength(512)]
        public string? FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        [StringLength(512)]
        public string? LastName { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [Column("EMail")]
        [StringLength(512)]
        public string? Email { get; set; }

        /// <summary>
        /// Password (encrypted)
        /// </summary>
        [StringLength(512)]
        public string Password { get; set; }

        /// <summary>
        /// Security code
        /// </summary>
        [StringLength(512)]
        public string SecurityCode { get; set; }

        /// <summary>
        /// Active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// right #1
        /// </summary>
        public long Right1 { get; set; }

        /// <summary>
        /// right #2
        /// </summary>
        public long Right2 { get; set; }

        /// <summary>
        /// right #3
        /// </summary>
        public long Right3 { get; set; }

        /// <summary>
        /// right #4
        /// </summary>
        public long Right4 { get; set; }

        /// <summary>
        /// right #5
        /// </summary>
        public long Right5 { get; set; }

        /// <summary>
        /// Last connection date
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? LastConnectionDate { get; set; }

        /// <summary>
        /// Creation date
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Update date
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime UpdateDate { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<Device> Device { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<UserSecurityCode> UserSecurityCode { get; set; }
    }
}
