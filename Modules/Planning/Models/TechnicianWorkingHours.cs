using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApi.Modules.Planning.Models
{
    [Table("technician_working_hours")]
    public class TechnicianWorkingHours
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("technician_id")]
        public int TechnicianId { get; set; }

        [Required]
        [Column("day_of_week")]
        public int DayOfWeek { get; set; } // 0 = Sunday, 6 = Saturday

        [Required]
        [Column("start_time")]
        public TimeSpan StartTime { get; set; }

        [Required]
        [Column("end_time")]
        public TimeSpan EndTime { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("effective_from")]
        public DateTime? EffectiveFrom { get; set; }

        [Column("effective_until")]
        public DateTime? EffectiveUntil { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
