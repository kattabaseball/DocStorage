using TourDocs.Domain.Common;
using TourDocs.Domain.Enums;

namespace TourDocs.Domain.Entities;

/// <summary>
/// A single item within a checklist specifying a required document and its constraints.
/// </summary>
public class ChecklistItem : BaseEntity
{
    public Guid ChecklistId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public DocumentCategory DocumentCategory { get; set; }
    public string? Description { get; set; }
    public string? FormatNotes { get; set; }
    public bool IsRequired { get; set; } = true;
    public bool RequiresOriginal { get; set; }
    public int? ValidityDays { get; set; }
    public int SortOrder { get; set; }

    // Navigation properties
    public virtual Checklist Checklist { get; set; } = null!;
}
