using System.ComponentModel.DataAnnotations;

namespace TourDocs.Core.DTOs.Organizations;

/// <summary>
/// Request DTO for creating a new organization.
/// </summary>
public class CreateOrganizationRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? BusinessRegNo { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [EmailAddress]
    [MaxLength(200)]
    public string? Email { get; set; }

    [MaxLength(300)]
    public string? Website { get; set; }

    [MaxLength(100)]
    public string? Industry { get; set; }
}
