using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace McpNetwork.Charli.Server.Models.Entities
{
    [Table("Plugins", Schema = "dbo")]
    public partial class Plugin
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PluginId { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [Required]
        [StringLength(512)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Filename
        /// </summary>
        [StringLength(255)]
        public string? FileName { get; set; }

        /// <summary>
        /// Version
        /// </summary>
        [Required]
        [StringLength(255)]
        public string? Version { get; set; }

        /// <summary>
        /// Active
        /// </summary>
        public bool Active { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdateDate { get; set; }
    }
}
