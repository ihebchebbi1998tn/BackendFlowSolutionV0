using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApi.Modules.ServiceOrders.Models
{
    [Table("service_order_jobs")]
    public class ServiceOrderJob
    {
        [Key]
        [Column("id")]
        [MaxLength(50)]
        public string Id { get; set; } = string.Empty;

        [Required]
        [Column("service_order_id")]
        [MaxLength(50)]
        public string ServiceOrderId { get; set; } = string.Empty;

        [Required]
        [Column("title")]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Required]
        [Column("status")]
        [MaxLength(20)]
        public string Status { get; set; } = "unscheduled";

        [Column("installation_id")]
        [MaxLength(50)]
        public string? InstallationId { get; set; }

        [Column("work_type")]
        [MaxLength(50)]
        public string? WorkType { get; set; }

        [Column("estimated_duration")]
        public int? EstimatedDuration { get; set; }

        [Column("estimated_cost", TypeName = "decimal(15,2)")]
        public decimal? EstimatedCost { get; set; } = 0;

        [Column("completion_percentage")]
        public int? CompletionPercentage { get; set; } = 0;

        [Column("assigned_technician_ids")]
        public string[]? AssignedTechnicianIds { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("ServiceOrderId")]
        public virtual ServiceOrder? ServiceOrder { get; set; }
    }
}
