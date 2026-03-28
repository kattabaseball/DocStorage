using TourDocs.Domain.Enums;

namespace TourDocs.Core.DTOs.HardCopy;

/// <summary>
/// Response DTO for a hard copy request.
/// </summary>
public class HardCopyRequestResponse
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public string DocumentTitle { get; set; } = string.Empty;
    public Guid CaseId { get; set; }
    public string CaseName { get; set; } = string.Empty;
    public Guid RequestedBy { get; set; }
    public string RequestedByName { get; set; } = string.Empty;
    public HardCopyStatus Status { get; set; }
    public Urgency Urgency { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public IReadOnlyList<HardCopyHandoverResponse> Handovers { get; set; } = Array.Empty<HardCopyHandoverResponse>();
}
