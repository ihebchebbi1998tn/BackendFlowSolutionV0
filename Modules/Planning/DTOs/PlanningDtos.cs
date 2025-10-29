using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyApi.Modules.Planning.DTOs
{
    // Assignment DTOs
    public class AssignJobDto
    {
        [Required]
        public string JobId { get; set; } = null!;

        [Required]
        [MinLength(1, ErrorMessage = "At least one technician must be assigned")]
        public List<string> TechnicianIds { get; set; } = new();

        [Required]
        public DateTime ScheduledDate { get; set; }

        [Required]
        public TimeSpan ScheduledStartTime { get; set; }

        [Required]
        public TimeSpan ScheduledEndTime { get; set; }

        public string Priority { get; set; } = "medium";

        public string? Notes { get; set; }

        public bool AutoCreateDispatch { get; set; } = true;
    }

    public class AssignJobResponseDto
    {
        public ServiceOrderJobDto Job { get; set; } = null!;
        public object? Dispatch { get; set; }
    }

    // Batch Assignment DTOs
    public class BatchAssignDto
    {
        [Required]
        [MinLength(1)]
        public List<AssignJobDto> Assignments { get; set; } = new();

        public bool AutoCreateDispatches { get; set; } = true;
    }

    public class BatchAssignResponseDto
    {
        public int Successful { get; set; }
        public int Failed { get; set; }
        public List<BatchAssignResult> Results { get; set; } = new();
    }

    public class BatchAssignResult
    {
        public string JobId { get; set; } = null!;
        public string Status { get; set; } = null!; // "success" or "failed"
        public string? DispatchId { get; set; }
        public string? ErrorMessage { get; set; }
    }

    // Validation DTOs
    public class ValidateAssignmentDto
    {
        public string JobId { get; set; } = null!;
        public List<string> TechnicianIds { get; set; } = new();
        public DateTime ScheduledDate { get; set; }
        public TimeSpan ScheduledStartTime { get; set; }
        public TimeSpan ScheduledEndTime { get; set; }
    }

    public class AssignmentValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Warnings { get; set; } = new();
        public List<AssignmentConflict> Conflicts { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
    }

    public class AssignmentConflict
    {
        public string Type { get; set; } = null!;
        public string Message { get; set; } = null!;
        public object? ConflictingData { get; set; }
    }

    // Schedule DTOs
    public class TechnicianScheduleDto
    {
        public string TechnicianId { get; set; } = null!;
        public string TechnicianName { get; set; } = null!;
        public Dictionary<string, WorkingHoursDto?> WorkingHours { get; set; } = new();
        public List<DispatchScheduleDto> Dispatches { get; set; } = new();
        public List<TechnicianLeaveDto> Leaves { get; set; } = new();
        public decimal TotalScheduledHours { get; set; }
        public decimal AvailableHours { get; set; }
    }

    public class WorkingHoursDto
    {
        public string Start { get; set; } = null!;
        public string End { get; set; } = null!;
    }

    public class DispatchScheduleDto
    {
        public string Id { get; set; } = null!;
        public string DispatchNumber { get; set; } = null!;
        public string JobId { get; set; } = null!;
        public string JobTitle { get; set; } = null!;
        public string ServiceOrderId { get; set; } = null!;
        public DateTime ScheduledDate { get; set; }
        public TimeSpan ScheduledStartTime { get; set; }
        public TimeSpan ScheduledEndTime { get; set; }
        public int EstimatedDuration { get; set; }
        public string Status { get; set; } = null!;
        public string Priority { get; set; } = null!;
        public LocationDto? Location { get; set; }
    }

    public class LocationDto
    {
        public string Address { get; set; } = null!;
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }

    public class TechnicianLeaveDto
    {
        public int Id { get; set; }
        public string LeaveType { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = null!;
    }

    // Service Order Job DTO
    public class ServiceOrderJobDto
    {
        public string Id { get; set; } = null!;
        public string ServiceOrderId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string Status { get; set; } = null!;
        public string Priority { get; set; } = null!;
        public int? EstimatedDuration { get; set; }
        public List<string>? RequiredSkills { get; set; }
        public List<string> AssignedTechnicianIds { get; set; } = new();
        public DateTime? ScheduledDate { get; set; }
        public TimeSpan? ScheduledStartTime { get; set; }
        public TimeSpan? ScheduledEndTime { get; set; }
        public LocationDto? Location { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    // Pagination DTOs
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public PaginationDto Pagination { get; set; } = new();
    }

    public class PaginationDto
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }

    // Technician Availability DTO
    public class TechnicianAvailabilityDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public List<string> Skills { get; set; } = new();
        public string Status { get; set; } = null!;
        public bool IsAvailable { get; set; }
        public int AvailableMinutes { get; set; }
        public int ScheduledMinutes { get; set; }
        public decimal UtilizationPercentage { get; set; }
    }
}
