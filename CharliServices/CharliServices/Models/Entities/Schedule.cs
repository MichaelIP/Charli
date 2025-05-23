using McpNetwork.Charli.Server.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace McpNetwork.Charli.Server.Models.Entities
{
    [Table("Schedules", Schema = "dbo")]
    public partial class Schedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ScheduleId { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [StringLength(255)]
        public string Name { get; set; }

        /// <summary>
        /// Active
        /// </summary>
        [Required]
        public bool Active { get; set; }

        /// <summary>
        /// Start date
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// End date
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Frequency
        /// </summary>
        public EScheduleFrequency Frequency { get; set; }

        /// <summary>
        /// Frequency every
        /// </summary>
        public int FrequencyEvery { get; set; }

        /// <summary>
        /// Start hour
        /// </summary>
        public int? StartHours { get; set; }

        /// <summary>
        /// Start days
        /// </summary>
        public int? StartDays { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdateDate { get; set; }
    }
}
