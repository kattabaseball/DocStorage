using TourDocs.Domain.Enums;

namespace TourDocs.Core.DTOs.DocumentRequests;

/// <summary>
/// Response DTO for a document request.
/// </summary>
public class DocumentRequestResponse
{
    public Guid Id { get; set; }
    public Guid? CaseId { get; set; }
    public string? CaseName { get; set; }
    public Guid MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public Guid RequestedBy { get; set; }
    public string RequestedByName { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string? FormatRequirements { get; set; }
    public Urgency Urgency { get; set; }
    public string? Notes { get; set; }
    public DocumentRequestStatus Status { get; set; }
    public Guid? FulfilledDocumentId { get; set; }
    public string? DeclineReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
