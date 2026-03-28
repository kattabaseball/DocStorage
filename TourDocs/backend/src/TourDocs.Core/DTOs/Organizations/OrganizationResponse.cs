using TourDocs.Domain.Enums;

namespace TourDocs.Core.DTOs.Organizations;

/// <summary>
/// Response DTO for a single organization.
/// </summary>
public class OrganizationResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? BusinessRegNo { get; set; }
    public string? LogoUrl { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? Industry { get; set; }
    public SubscriptionPlan SubscriptionPlan { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
