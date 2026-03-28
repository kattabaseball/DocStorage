using TourDocs.Domain.Enums;

namespace TourDocs.Core.DTOs.Checklists;

/// <summary>
/// Response DTO for a single checklist item.
/// </summary>
public class ChecklistItemResponse
{
    public Guid Id { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public DocumentCategory DocumentCategory { get; set; }
    public string? Description { get; set; }
    public string? FormatNotes { get; set; }
    public bool IsRequired { get; set; }
    public bool RequiresOriginal { get; set; }
    public int? ValidityDays { get; set; }
    public int SortOrder { get; set; }
}
