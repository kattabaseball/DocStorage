using System.ComponentModel.DataAnnotations;

namespace TourDocs.Core.DTOs.HardCopy;

/// <summary>
/// Request DTO for recording a hard copy handover.
/// </summary>
public class CreateHardCopyHandoverRequest
{
    [Required]
    public Guid ToUserId { get; set; }

    [Required]
    [MaxLength(50)]
    public string HandoverType { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? ConfirmationMethod { get; set; }

    [MaxLength(1000)]
    public string? ConfirmationData { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }
}
