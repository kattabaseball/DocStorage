using TourDocs.Domain.Common;
using TourDocs.Domain.Enums;

namespace TourDocs.Domain.Entities;

/// <summary>
/// Manages access control for external users or specific team members to a case.
/// </summary>
public class CaseAccess : BaseEntity
{
    public Guid CaseId { get; set; }
    public Guid UserId { get; set; }
    public UserRole Role { get; set; }
    public CaseAccessPermission Permission { get; set; }
    public Guid GrantedBy { get; set; }
    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? RevokedAt { get; set; }
    public Guid? RevokedBy { get; set; }

    // Navigation properties
    public virtual Case Case { get; set; } = null!;
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual ApplicationUser GrantedByUser { get; set; } = null!;
}
