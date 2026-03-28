using System.Security.Claims;
using TourDocs.Domain.Entities;

namespace TourDocs.Core.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(ApplicationUser user, Guid? organizationId, string role);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
    DateTime GetAccessTokenExpiration();
    DateTime GetRefreshTokenExpiration();
}
