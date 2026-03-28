using TourDocs.Core.DTOs.Subscriptions;

namespace TourDocs.Core.Interfaces;

/// <summary>
/// Service contract for subscription management operations.
/// </summary>
public interface ISubscriptionService
{
    Task<SubscriptionResponse> GetByOrganizationAsync(Guid organizationId);
    Task<SubscriptionResponse> UpdatePlanAsync(Guid organizationId, UpdateSubscriptionRequest request);
    Task<SubscriptionUsageResponse> GetUsageAsync(Guid organizationId);
}
