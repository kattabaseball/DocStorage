using TourDocs.Domain.Common;
using TourDocs.Domain.Enums;

namespace TourDocs.Domain.Entities;

/// <summary>
/// Represents an organization's subscription with plan details and billing information.
/// </summary>
public class Subscription : BaseEntity
{
    public Guid OrganizationId { get; set; }
    public SubscriptionPlan Plan { get; set; }
    public string Status { get; set; } = "Active";
    public int MaxMembers { get; set; }
    public int MaxCasesMonthly { get; set; }
    public int MaxExternalUsers { get; set; }
    public long MaxStorageBytes { get; set; }
    public string? PaymentGatewayCustomerId { get; set; }
    public string? PaymentGatewaySubscriptionId { get; set; }
    public DateTime CurrentPeriodStart { get; set; }
    public DateTime CurrentPeriodEnd { get; set; }

    // Navigation properties
    public virtual Organization Organization { get; set; } = null!;
}
