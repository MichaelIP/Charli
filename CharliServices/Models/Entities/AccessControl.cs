using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace McpNetwork.Charli.Server.Models.Entities
{
    [Table("AccessControls", Schema = "dbo")]
    public partial class AccessControl
    {
        /// <summary>
        /// Key
        /// </summary>

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AccessControlId { get; set; }

        /// <summary>
        /// Category 
        /// </summary>
        public string? Category { get; set; }

        [Required]
        public string? Name { get; set; }

        /// <summary>
        /// Set #
        /// </summary>
        public byte SetNb { get; set; }

        /// <summary>
        /// Bit #
        /// </summary>
        public byte BitNb { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdateDate { get; set; }
    }
}
