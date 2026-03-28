using TourDocs.Domain.Enums;

namespace TourDocs.Core.DTOs.Organizations;

/// <summary>
/// Response DTO for an organization member (user within an organization).
/// </summary>
public class OrganizationMemberResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime? JoinedAt { get; set; }
    public bool IsActive { get; set; }
}
