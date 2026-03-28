using TourDocs.Domain.Common;
using TourDocs.Domain.Enums;

namespace TourDocs.Domain.Entities;

/// <summary>
/// Represents a document managed by the platform, supporting versioning and verification workflows.
/// </summary>
public class Document : AuditableEntity, ISoftDelete
{
    public Guid MemberId { get; set; }
    public Guid OrganizationId { get; set; }
    public DocumentCategory Category { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DocumentStatus Status { get; set; } = DocumentStatus.Uploaded;
    public Guid? CurrentVersionId { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsHardCopyNeeded { get; set; }
    public string? ExtractedData { get; set; }
    public string? VerificationNotes { get; set; }
    public Guid? VerifiedBy { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public virtual Member Member { get; set; } = null!;
    public virtual Organization Organization { get; set; } = null!;
    public virtual ICollection<DocumentVersion> Versions { get; set; } = new List<DocumentVersion>();
    public virtual DocumentVersion? CurrentVersion { get; set; }
}
