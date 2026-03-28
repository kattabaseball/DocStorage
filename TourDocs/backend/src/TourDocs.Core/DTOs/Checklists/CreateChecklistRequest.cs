using System.ComponentModel.DataAnnotations;

namespace TourDocs.Core.DTOs.Checklists;

/// <summary>
/// Request DTO for creating a new checklist.
/// </summary>
public class CreateChecklistRequest
{
    [MaxLength(10)]
    public string? CountryCode { get; set; }

    [MaxLength(100)]
    public string? CountryName { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? ChecklistType { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }

    public List<CreateChecklistItemRequest> Items { get; set; } = new();
}
