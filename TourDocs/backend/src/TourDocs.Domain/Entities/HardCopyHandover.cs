using TourDocs.Domain.Common;

namespace TourDocs.Domain.Entities;

/// <summary>
/// Records each handover event in the chain of custody for a hard copy document.
/// </summary>
public class HardCopyHandover : BaseEntity
{
    public Guid HardCopyRequestId { get; set; }
    public Guid FromUserId { get; set; }
    public Guid ToUserId { get; set; }
    public string FromRole { get; set; } = string.Empty;
    public string ToRole { get; set; } = string.Empty;
    public string HandoverType { get; set; } = string.Empty;
    public string? ConfirmationMethod { get; set; }
    public string? ConfirmationData { get; set; }
    public string? Notes { get; set; }
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    public Guid RecordedBy { get; set; }

    // Navigation properties
    public virtual HardCopyRequest HardCopyRequest { get; set; } = null!;
    public virtual ApplicationUser FromUser { get; set; } = null!;
    public virtual ApplicationUser ToUser { get; set; } = null!;
}
