namespace TourDocs.Core.Interfaces;

/// <summary>
/// Provides the current tenant (organization) context for multi-tenancy filtering.
/// </summary>
public interface ITenantContext
{
    Guid OrganizationId { get; }
    Guid UserId { get; }
}
