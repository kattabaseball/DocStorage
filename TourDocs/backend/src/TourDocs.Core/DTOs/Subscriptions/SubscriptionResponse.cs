using TourDocs.Domain.Enums;

namespace TourDocs.Core.DTOs.Subscriptions;

/// <summary>
/// Response DTO for subscription details.
/// </summary>
public class SubscriptionResponse
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public SubscriptionPlan Plan { get; set; }
    public string Status { get; set; } = string.Empty;
    public int MaxMembers { get; set; }
    public int MaxCasesMonthly { get; set; }
    public int MaxExternalUsers { get; set; }
    public long MaxStorageBytes { get; set; }
    public DateTime CurrentPeriodStart { get; set; }
    public DateTime CurrentPeriodEnd { get; set; }
    public DateTime CreatedAt { get; set; }
}
