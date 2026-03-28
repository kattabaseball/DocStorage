using System.ComponentModel.DataAnnotations;
using TourDocs.Domain.Enums;

namespace TourDocs.Core.DTOs.HardCopy;

/// <summary>
/// Request DTO for creating a hard copy request.
/// </summary>
public class CreateHardCopyRequestDto
{
    [Required]
    public Guid DocumentId { get; set; }

    [Required]
    public Guid CaseId { get; set; }

    public Urgency Urgency { get; set; } = Urgency.Normal;

    [MaxLength(2000)]
    public string? Notes { get; set; }
}
