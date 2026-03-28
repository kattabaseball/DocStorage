using System.ComponentModel.DataAnnotations;
using TourDocs.Domain.Enums;

namespace TourDocs.Core.DTOs.Checklists;

/// <summary>
/// Request DTO for adding an item to a checklist.
/// </summary>
public class CreateChecklistItemRequest
{
    [Required]
    [MaxLength(100)]
    public string DocumentType { get; set; } = string.Empty;

    public DocumentCategory DocumentCategory { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(500)]
    public string? FormatNotes { get; set; }

    public bool IsRequired { get; set; } = true;
    public bool RequiresOriginal { get; set; }
    public int? ValidityDays { get; set; }
    public int SortOrder { get; set; }
}
