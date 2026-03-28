using System.ComponentModel.DataAnnotations;

namespace TourDocs.Core.DTOs.Auth;

/// <summary>
/// Request DTO for user registration.
/// </summary>
public class RegisterRequest
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string OrganizationName { get; set; } = string.Empty;
}
