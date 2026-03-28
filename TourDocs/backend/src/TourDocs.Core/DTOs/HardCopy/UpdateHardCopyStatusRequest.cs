using System.ComponentModel.DataAnnotations;
using TourDocs.Domain.Enums;

namespace TourDocs.Core.DTOs.HardCopy;

/// <summary>
/// Request DTO for updating a hard copy request status.
/// </summary>
public class UpdateHardCopyStatusRequest
{
    [Required]
    public HardCopyStatus Status { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }
}
