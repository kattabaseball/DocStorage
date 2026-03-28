using TourDocs.Domain.Common;

namespace TourDocs.Domain.Entities;

/// <summary>
/// Represents a refresh token stored in the database for secure token rotation.
/// </summary>
public class RefreshToken : BaseEntity
{
    public string Token { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime? RevokedAt { get; set; }

    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
}
