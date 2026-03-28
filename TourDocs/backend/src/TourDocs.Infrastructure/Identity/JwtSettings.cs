namespace TourDocs.Infrastructure.Identity;

/// <summary>
/// Configuration settings for JWT token generation and validation.
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Secret key used for signing JWT tokens. Must be at least 256 bits.
    /// </summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// JWT token issuer.
    /// </summary>
    public string Issuer { get; set; } = "TourDocs";

    /// <summary>
    /// JWT token audience.
    /// </summary>
    public string Audience { get; set; } = "TourDocs";

    /// <summary>
    /// Access token expiration in minutes.
    /// </summary>
    public int AccessTokenExpirationMinutes { get; set; } = 15;

    /// <summary>
    /// Refresh token expiration in days.
    /// </summary>
    public int RefreshTokenExpirationDays { get; set; } = 7;
}
