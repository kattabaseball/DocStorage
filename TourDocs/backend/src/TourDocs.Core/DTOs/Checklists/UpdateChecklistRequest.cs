using System.ComponentModel.DataAnnotations;

namespace TourDocs.Core.DTOs.Checklists;

/// <summary>
/// Request DTO for updating an existing checklist.
/// </summary>
public class UpdateChecklistRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? ChecklistType { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }

    public bool IsActive { get; set; } = true;
}
