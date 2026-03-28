using TourDocs.Domain.Common;

namespace TourDocs.Domain.Entities;

/// <summary>
/// Links a member to a case with status tracking.
/// </summary>
public class CaseMember : BaseEntity
{
    public Guid CaseId { get; set; }
    public Guid MemberId { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Case Case { get; set; } = null!;
    public virtual Member Member { get; set; } = null!;
}
