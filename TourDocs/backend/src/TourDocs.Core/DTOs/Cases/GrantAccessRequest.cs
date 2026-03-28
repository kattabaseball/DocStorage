using TourDocs.Domain.Enums;

namespace TourDocs.Core.DTOs.Cases;

/// <summary>
/// Request DTO for granting case access to a user.
/// </summary>
public class GrantAccessRequest
{
    public Guid UserId { get; set; }
    public UserRole Role { get; set; }
    public CaseAccessPermission Permission { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
