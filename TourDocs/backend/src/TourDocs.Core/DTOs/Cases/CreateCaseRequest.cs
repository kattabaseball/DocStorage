namespace TourDocs.Core.DTOs.Cases;

/// <summary>
/// Request DTO for creating a new case.
/// </summary>
public class CreateCaseRequest
{
    public string Name { get; set; } = string.Empty;
    public string? CaseType { get; set; }
    public string? DestinationCountry { get; set; }
    public string? DestinationCity { get; set; }
    public string? Venue { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? ContactName { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public Guid? ChecklistId { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
}
