namespace TourDocs.Core.Interfaces;

/// <summary>
/// Provides access to the currently authenticated user's information.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// The unique identifier of the current user.
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// The organization identifier from the current user's claims.
    /// </summary>
    Guid? OrganizationId { get; }

    /// <summary>
    /// The email address of the current user.
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// The role of the current user within their organization.
    /// </summary>
    string? Role { get; }

    /// <summary>
    /// Whether a user is currently authenticated.
    /// </summary>
    bool IsAuthenticated { get; }
}
