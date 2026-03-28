namespace TourDocs.Core.DTOs.Cases;

/// <summary>
/// Request DTO for updating an existing case.
/// </summary>
public class UpdateCaseRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CaseType { get; set; }
    public string? DestinationCountry { get; set; }
    public string? DestinationCity { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid? ChecklistId { get; set; }
    public string? ContactName { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? Notes { get; set; }
}
