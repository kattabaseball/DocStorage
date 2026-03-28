using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TourDocs.Core.DTOs.Auth;
using TourDocs.Core.Exceptions;
using TourDocs.Core.Interfaces;
using TourDocs.Domain.Entities;
using TourDocs.Domain.Interfaces;
using TourDocs.Domain.Enums;

namespace TourDocs.Core.Services;

/// <summary>
/// Business logic service for authentication and token management.
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AuthService> _logger;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IEmailService _emailService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork,
        ILogger<AuthService> logger,
        IJwtTokenService jwtTokenService,
        IEmailService emailService)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _jwtTokenService = jwtTokenService;
        _emailService = emailService;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new Exceptions.ValidationException("Credentials", "Invalid email or password.");
        }

        if (!user.IsActive)
        {
            throw new ForbiddenException("Your account has been deactivated.");
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        var orgQuery = _unitOfWork.Organizations.Query()
            .SelectMany(o => o.OrganizationMembers.Where(om => om.UserId == user.Id && om.IsActive))
            .Select(om => new { om.OrganizationId, om.Organization.Name, om.Role });
        var orgMembership = await FirstOrDefaultAsync(orgQuery);

        var role = orgMembership?.Role.ToString() ?? UserRole.Member.ToString();
        var refreshTokenValue = _jwtTokenService.GenerateRefreshToken();

        // Store the refresh token in the database
        await StoreRefreshTokenAsync(user.Id, refreshTokenValue);

        var response = new AuthResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            OrganizationId = orgMembership?.OrganizationId,
            OrganizationName = orgMembership?.Name,
            Role = role,
            AccessToken = _jwtTokenService.GenerateAccessToken(user, orgMembership?.OrganizationId, role),
            RefreshToken = refreshTokenValue,
            ExpiresAt = _jwtTokenService.GetAccessTokenExpiration()
        };

        _logger.LogInformation("User {UserId} logged in successfully", user.Id);

        return response;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new Exceptions.ValidationException("Email", "A user with this email already exists.");
        }

        var fullName = $"{request.FirstName} {request.LastName}";

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = request.Email,
            Email = request.Email,
            FullName = fullName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.ToDictionary(
                e => e.Code,
                e => new[] { e.Description });
            throw new Exceptions.ValidationException("Registration failed.", errors);
        }

        var slug = request.OrganizationName.ToLowerInvariant()
            .Replace(" ", "-")
            .Where(c => char.IsLetterOrDigit(c) || c == '-')
            .Aggregate(string.Empty, (current, c) => current + c);

        var organization = new Organization
        {
            Id = Guid.NewGuid(),
            Name = request.OrganizationName,
            Slug = $"{slug}-{Guid.NewGuid().ToString("N")[..6]}",
            SubscriptionPlan = SubscriptionPlan.Starter,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Organizations.AddAsync(organization);

        // Create OrganizationMember linking the user to the organization as OrgOwner
        var organizationMember = new OrganizationMember
        {
            Id = Guid.NewGuid(),
            OrganizationId = organization.Id,
            UserId = user.Id,
            Role = UserRole.OrgOwner,
            InvitedAt = DateTime.UtcNow,
            JoinedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _unitOfWork.OrganizationMembers.AddAsync(organizationMember);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("User {UserId} registered with organization {OrgId}", user.Id, organization.Id);

        var registerRole = UserRole.OrgOwner.ToString();
        var refreshTokenValue = _jwtTokenService.GenerateRefreshToken();

        // Store the refresh token in the database
        await StoreRefreshTokenAsync(user.Id, refreshTokenValue);

        return new AuthResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            OrganizationId = organization.Id,
            OrganizationName = organization.Name,
            Role = registerRole,
            AccessToken = _jwtTokenService.GenerateAccessToken(user, organization.Id, registerRole),
            RefreshToken = refreshTokenValue,
            ExpiresAt = _jwtTokenService.GetAccessTokenExpiration()
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        // Validate the expired access token to extract user identity
        var principal = _jwtTokenService.ValidateToken(request.AccessToken);
        if (principal == null)
        {
            throw new Exceptions.ValidationException("AccessToken", "Invalid access token.");
        }

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new Exceptions.ValidationException("AccessToken", "Invalid access token claims.");
        }

        // Find the stored refresh token
        var storedToken = await _unitOfWork.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken
                                       && rt.UserId == userId
                                       && !rt.IsRevoked);

        if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new Exceptions.ValidationException("RefreshToken", "Invalid or expired refresh token.");
        }

        // Revoke the old refresh token (token rotation)
        storedToken.IsRevoked = true;
        storedToken.RevokedAt = DateTime.UtcNow;
        _unitOfWork.RefreshTokens.Update(storedToken);

        // Load the user
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User", userId);

        if (!user.IsActive)
        {
            throw new ForbiddenException("Your account has been deactivated.");
        }

        // Look up organization membership
        var orgQuery = _unitOfWork.Organizations.Query()
            .SelectMany(o => o.OrganizationMembers.Where(om => om.UserId == user.Id && om.IsActive))
            .Select(om => new { om.OrganizationId, om.Organization.Name, om.Role });
        var orgMembership = await FirstOrDefaultAsync(orgQuery);

        var role = orgMembership?.Role.ToString() ?? UserRole.Member.ToString();

        // Generate new tokens
        var newRefreshTokenValue = _jwtTokenService.GenerateRefreshToken();
        await StoreRefreshTokenAsync(user.Id, newRefreshTokenValue);

        _logger.LogInformation("Token refreshed for user {UserId}", user.Id);

        return new AuthResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            OrganizationId = orgMembership?.OrganizationId,
            OrganizationName = orgMembership?.Name,
            Role = role,
            AccessToken = _jwtTokenService.GenerateAccessToken(user, orgMembership?.OrganizationId, role),
            RefreshToken = newRefreshTokenValue,
            ExpiresAt = _jwtTokenService.GetAccessTokenExpiration()
        };
    }

    public async Task LogoutAsync(Guid userId)
    {
        // Revoke all active refresh tokens for this user
        var activeTokens = await _unitOfWork.RefreshTokens
            .FindAsync(rt => rt.UserId == userId && !rt.IsRevoked);

        foreach (var token in activeTokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            _unitOfWork.RefreshTokens.Update(token);
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("User {UserId} logged out, all refresh tokens revoked", userId);
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User", userId);

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            var errors = result.Errors.ToDictionary(
                e => e.Code,
                e => new[] { e.Description });
            throw new Exceptions.ValidationException("Password change failed.", errors);
        }

        _logger.LogInformation("Password changed for user {UserId}", userId);
    }

    public async Task<UserProfileResponse> GetCurrentUserAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User", userId);

        var orgQuery = _unitOfWork.Organizations.Query()
            .SelectMany(o => o.OrganizationMembers.Where(om => om.UserId == user.Id && om.IsActive))
            .Select(om => new { om.OrganizationId, om.Organization.Name, om.Role });
        var orgMembership = await FirstOrDefaultAsync(orgQuery);

        var nameParts = (user.FullName ?? string.Empty).Split(' ', 2);

        return new UserProfileResponse
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = nameParts.Length > 0 ? nameParts[0] : string.Empty,
            LastName = nameParts.Length > 1 ? nameParts[1] : string.Empty,
            FullName = user.FullName,
            AvatarUrl = user.AvatarUrl,
            Roles = orgMembership != null
                ? new List<string> { orgMembership.Role.ToString() }
                : new List<string>(),
            OrganizationId = orgMembership?.OrganizationId,
            OrganizationName = orgMembership?.Name,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        };
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        // Always return success to prevent email enumeration attacks
        if (user == null || !user.IsActive)
        {
            _logger.LogWarning("Password reset requested for unknown or inactive email {Email}", request.Email);
            return;
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = $"https://app.tourdocs.com/auth/reset-password?email={Uri.EscapeDataString(user.Email!)}&token={Uri.EscapeDataString(token)}";

        await _emailService.SendPasswordResetAsync(user.Email!, user.FullName, resetLink);

        _logger.LogInformation("Password reset email sent for user {UserId}", user.Id);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email)
            ?? throw new Exceptions.ValidationException("Email", "Invalid password reset request.");

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!result.Succeeded)
        {
            var errors = result.Errors.ToDictionary(
                e => e.Code,
                e => new[] { e.Description });
            throw new Exceptions.ValidationException("Password reset failed.", errors);
        }

        // Revoke all existing refresh tokens after password reset
        var activeTokens = await _unitOfWork.RefreshTokens
            .FindAsync(rt => rt.UserId == user.Id && !rt.IsRevoked);

        foreach (var token in activeTokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            _unitOfWork.RefreshTokens.Update(token);
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Password reset completed for user {UserId}", user.Id);
    }

    /// <summary>
    /// Stores a new refresh token in the database for the specified user.
    /// </summary>
    private async Task StoreRefreshTokenAsync(Guid userId, string tokenValue)
    {
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = tokenValue,
            UserId = userId,
            ExpiresAt = _jwtTokenService.GetRefreshTokenExpiration(),
            IsRevoked = false
        };

        await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
        await _unitOfWork.SaveChangesAsync();
    }

    private static async Task<T?> FirstOrDefaultAsync<T>(IQueryable<T> query)
    {
        return await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(query);
    }
}
