using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TourDocs.Core.Interfaces;

namespace TourDocs.Infrastructure.Identity;

/// <summary>
/// Extracts the current user's identity from the HTTP context claims.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    public Guid? OrganizationId
    {
        get
        {
            var orgIdClaim = _httpContextAccessor.HttpContext?.User
                .FindFirstValue("org_id");
            return Guid.TryParse(orgIdClaim, out var orgId) ? orgId : null;
        }
    }

    public string? Email =>
        _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);

    public string? Role =>
        _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}
