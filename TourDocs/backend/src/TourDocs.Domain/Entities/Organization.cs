using TourDocs.Domain.Common;
using TourDocs.Domain.Enums;

namespace TourDocs.Domain.Entities;

/// <summary>
/// Represents an organization that manages members, documents, and cases.
/// </summary>
public class Organization : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? BusinessRegNo { get; set; }
    public string? LogoUrl { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? Industry { get; set; }
    public SubscriptionPlan SubscriptionPlan { get; set; } = SubscriptionPlan.Starter;
    public bool IsActive { get; set; } = true;

    // Preferences
    public string Language { get; set; } = "en";
    public string Timezone { get; set; } = "UTC";
    public bool EmailNotifications { get; set; } = true;
    public bool ExpiryReminders { get; set; } = true;

    // Navigation properties
    public virtual ICollection<Member> Members { get; set; } = new List<Member>();
    public virtual ICollection<OrganizationMember> OrganizationMembers { get; set; } = new List<OrganizationMember>();
    public virtual ICollection<Case> Cases { get; set; } = new List<Case>();
    public virtual Subscription? Subscription { get; set; }
}
