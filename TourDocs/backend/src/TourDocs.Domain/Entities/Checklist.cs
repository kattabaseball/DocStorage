using TourDocs.Domain.Common;

namespace TourDocs.Domain.Entities;

/// <summary>
/// Defines a checklist of required documents, optionally tied to a country or organization.
/// </summary>
public class Checklist : BaseEntity
{
    public string? CountryCode { get; set; }
    public string? CountryName { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ChecklistType { get; set; }
    public int Version { get; set; } = 1;
    public bool IsSystem { get; set; }
    public Guid? OrganizationId { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual Organization? Organization { get; set; }
    public virtual ICollection<ChecklistItem> Items { get; set; } = new List<ChecklistItem>();
    public virtual ICollection<Case> Cases { get; set; } = new List<Case>();
}
