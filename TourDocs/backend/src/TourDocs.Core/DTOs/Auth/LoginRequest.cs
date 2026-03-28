using System.ComponentModel.DataAnnotations;

namespace TourDocs.Core.DTOs.Auth;

/// <summary>
/// Request DTO for user login.
/// </summary>
public class LoginRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}
