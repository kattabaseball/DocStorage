using TourDocs.Domain.Common;
using TourDocs.Domain.Enums;

namespace TourDocs.Domain.Entities;

/// <summary>
/// Tracks requests for physical (hard copy) documents and their chain-of-custody status.
/// </summary>
public class HardCopyRequest : BaseEntity
{
    public Guid DocumentId { get; set; }
    public Guid CaseId { get; set; }
    public Guid RequestedBy { get; set; }
    public HardCopyStatus Status { get; set; } = HardCopyStatus.Requested;
    public Urgency Urgency { get; set; } = Urgency.Normal;
    public string? Notes { get; set; }

    // Navigation properties
    public virtual Document Document { get; set; } = null!;
    public virtual Case Case { get; set; } = null!;
    public virtual ApplicationUser RequestedByUser { get; set; } = null!;
    public virtual ICollection<HardCopyHandover> Handovers { get; set; } = new List<HardCopyHandover>();
}
