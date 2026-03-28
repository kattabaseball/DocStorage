using System.Security.Claims;

namespace TourDocs.API.Middleware;

/// <summary>
/// Middleware that extracts the organization context from JWT claims and makes it available
/// to downstream services via the request context.
/// </summary>
public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantMiddleware> _logger;

    public TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var orgIdClaim = context.User.FindFirstValue("org_id");
            if (!string.IsNullOrEmpty(orgIdClaim) && Guid.TryParse(orgIdClaim, out var organizationId))
            {
                context.Items["OrganizationId"] = organizationId;
                _logger.LogDebug("Tenant context set: OrgId={OrganizationId}", organizationId);
            }

            var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
            {
                context.Items["UserId"] = userId;
            }
        }

        await _next(context);
    }
}
