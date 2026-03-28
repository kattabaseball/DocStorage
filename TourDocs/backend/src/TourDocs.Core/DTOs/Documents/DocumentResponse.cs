using TourDocs.Domain.Enums;

namespace TourDocs.Core.DTOs.Documents;

/// <summary>
/// Response DTO for a single document.
/// </summary>
public class DocumentResponse
{
    public Guid Id { get; set; }
    public Guid MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public Guid OrganizationId { get; set; }
    public DocumentCategory Category { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DocumentStatus Status { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsHardCopyNeeded { get; set; }
    public string? VerificationNotes { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public int VersionCount { get; set; }
    public int CurrentVersionNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
