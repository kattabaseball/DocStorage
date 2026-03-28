namespace TourDocs.Core.DTOs.Checklists;

/// <summary>
/// Response DTO for a checklist with its items.
/// </summary>
public class ChecklistResponse
{
    public Guid Id { get; set; }
    public string? CountryCode { get; set; }
    public string? CountryName { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ChecklistType { get; set; }
    public int Version { get; set; }
    public bool IsSystem { get; set; }
    public Guid? OrganizationId { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public IReadOnlyList<ChecklistItemResponse> Items { get; set; } = Array.Empty<ChecklistItemResponse>();
}
