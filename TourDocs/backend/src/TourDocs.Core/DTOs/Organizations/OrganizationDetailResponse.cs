using TourDocs.Core.DTOs.Subscriptions;
using TourDocs.Domain.Enums;

namespace TourDocs.Core.DTOs.Organizations;

/// <summary>
/// Detailed response DTO for an organization including member list and subscription info.
/// </summary>
public class OrganizationDetailResponse : OrganizationResponse
{
    public int MemberCount { get; set; }
    public int UserCount { get; set; }
    public int CaseCount { get; set; }
    public SubscriptionResponse? Subscription { get; set; }
    public IReadOnlyList<OrganizationMemberResponse> Users { get; set; } = Array.Empty<OrganizationMemberResponse>();
}
