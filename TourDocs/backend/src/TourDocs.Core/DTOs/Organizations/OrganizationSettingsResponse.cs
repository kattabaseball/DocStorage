namespace TourDocs.Core.DTOs.Organizations;

/// <summary>
/// Response DTO for organization settings (used by the settings page).
/// </summary>
public class OrganizationSettingsResponse
{
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string Language { get; set; } = "en";
    public string Timezone { get; set; } = "UTC";
    public bool EmailNotifications { get; set; } = true;
    public bool ExpiryReminders { get; set; } = true;
}
