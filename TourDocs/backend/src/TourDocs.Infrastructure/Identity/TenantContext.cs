using TourDocs.Core.Interfaces;

namespace TourDocs.Infrastructure.Identity;

/// <summary>
/// Provides the current tenant context extracted from JWT claims.
/// </summary>
public class TenantContext : ITenantContext
{
    private readonly ICurrentUserService _currentUserService;

    public TenantContext(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public Guid OrganizationId =>
        _currentUserService.OrganizationId ?? Guid.Empty;

    public Guid UserId =>
        _currentUserService.UserId ?? Guid.Empty;
}
