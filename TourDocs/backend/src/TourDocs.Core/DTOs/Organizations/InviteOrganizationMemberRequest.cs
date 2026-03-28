using System.ComponentModel.DataAnnotations;
using TourDocs.Domain.Enums;

namespace TourDocs.Core.DTOs.Organizations;

/// <summary>
/// Request DTO for inviting a user to an organization.
/// </summary>
public class InviteOrganizationMemberRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; }
}
