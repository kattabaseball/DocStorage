namespace TourDocs.Domain.Entities;

/// <summary>
/// Records an audit trail entry for tracking all significant system actions.
/// </summary>
public class AuditLog
{
    public Guid Id { get; set; }
    public Guid? OrganizationId { get; set; }
    public Guid? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public string? Details { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Organization? Organization { get; set; }
    public virtual ApplicationUser? User { get; set; }
}
