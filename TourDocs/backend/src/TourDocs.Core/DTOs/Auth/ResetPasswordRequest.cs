using System.ComponentModel.DataAnnotations;

namespace TourDocs.Core.DTOs.Auth;

/// <summary>
/// Request DTO for resetting a user's password with a valid reset token.
/// </summary>
public class ResetPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string NewPassword { get; set; } = string.Empty;
}
