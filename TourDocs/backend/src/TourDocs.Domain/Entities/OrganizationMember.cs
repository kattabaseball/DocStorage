using TourDocs.Domain.Common;
using TourDocs.Domain.Enums;

namespace TourDocs.Domain.Entities;

/// <summary>
/// Represents the many-to-many relationship between users and organizations with role assignment.
/// </summary>
public class OrganizationMember : BaseEntity
{
    public Guid OrganizationId { get; set; }
    public Guid UserId { get; set; }
    public UserRole Role { get; set; }
    public DateTime InvitedAt { get; set; } = DateTime.UtcNow;
    public DateTime? JoinedAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual Organization Organization { get; set; } = null!;
    public virtual ApplicationUser User { get; set; } = null!;
}
