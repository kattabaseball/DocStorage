using System.ComponentModel.DataAnnotations;

namespace TourDocs.Core.DTOs.Auth;

/// <summary>
/// Request DTO for initiating a password reset.
/// </summary>
public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
