using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace McpNetwork.Charli.Server.Models.Entities
{
    [Table("UserSecurityCodes", Schema = "dbo")]
    public partial class UserSecurityCode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserSecurityCodeId { get; set; }

        /// <summary>
        /// User #
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        [Required]
        [StringLength(400)]
        public string Token { get; set; }

        /// <summary>
        /// Token expiration date
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime ExpirationDate { get; set; }

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

        [ForeignKey("UserId")]
        [InverseProperty("UserSecurityCode")]
        public virtual User User { get; set; }
    }
}
