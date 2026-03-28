using TourDocs.Domain.Enums;

namespace TourDocs.Core.DTOs.Cases;

/// <summary>
/// Response DTO for a single case.
/// </summary>
public class CaseResponse
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? CaseType { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? DestinationCountry { get; set; }
    public string? DestinationCity { get; set; }
    public string? Venue { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? ContactName { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public Guid? ChecklistId { get; set; }
    public CaseStatus Status { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public int MemberCount { get; set; }
    public double ReadinessPercentage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
