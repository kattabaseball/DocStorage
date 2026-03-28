using System.ComponentModel.DataAnnotations;

namespace TourDocs.Core.DTOs.Auth;

/// <summary>
/// Request DTO for changing user password.
/// </summary>
public class ChangePasswordRequest
{
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string NewPassword { get; set; } = string.Empty;
}
