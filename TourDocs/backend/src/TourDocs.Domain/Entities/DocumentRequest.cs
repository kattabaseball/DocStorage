using TourDocs.Domain.Common;
using TourDocs.Domain.Enums;

namespace TourDocs.Domain.Entities;

/// <summary>
/// Represents a request for a specific document from a member, optionally linked to a case.
/// </summary>
public class DocumentRequest : BaseEntity
{
    public Guid? CaseId { get; set; }
    public Guid MemberId { get; set; }
    public Guid RequestedBy { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string? FormatRequirements { get; set; }
    public Urgency Urgency { get; set; } = Urgency.Normal;
    public string? Notes { get; set; }
    public DocumentRequestStatus Status { get; set; } = DocumentRequestStatus.Requested;
    public Guid? FulfilledDocumentId { get; set; }
    public string? DeclineReason { get; set; }

    // Navigation properties
    public virtual Case? Case { get; set; }
    public virtual Member Member { get; set; } = null!;
    public virtual ApplicationUser RequestedByUser { get; set; } = null!;
    public virtual Document? FulfilledDocument { get; set; }
}
