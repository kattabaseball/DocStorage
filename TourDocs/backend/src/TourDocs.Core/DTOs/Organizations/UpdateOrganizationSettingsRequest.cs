using System.ComponentModel.DataAnnotations;

namespace TourDocs.Core.DTOs.Organizations;

/// <summary>
/// Request DTO for updating organization settings from the settings page.
/// </summary>
public class UpdateOrganizationSettingsRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [EmailAddress]
    [MaxLength(200)]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(10)]
    public string Language { get; set; } = "en";

    [MaxLength(100)]
    public string Timezone { get; set; } = "UTC";

    public bool EmailNotifications { get; set; } = true;

    public bool ExpiryReminders { get; set; } = true;
}
