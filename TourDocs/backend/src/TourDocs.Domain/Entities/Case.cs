using TourDocs.Domain.Common;
using TourDocs.Domain.Enums;

namespace TourDocs.Domain.Entities;

/// <summary>
/// Represents a case (e.g., a visa application, travel arrangement) that groups members and documents.
/// </summary>
public class Case : AuditableEntity, ISoftDelete
{
    public Guid OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? CaseType { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? DestinationCountry { get; set; }
    public string? DestinationCity { get; set; }
    public string? Venue { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? ContactName { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public Guid? ChecklistId { get; set; }
    public CaseStatus Status { get; set; } = CaseStatus.Draft;
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public virtual Organization Organization { get; set; } = null!;
    public virtual Checklist? Checklist { get; set; }
    public virtual ICollection<CaseMember> CaseMembers { get; set; } = new List<CaseMember>();
    public virtual ICollection<CaseAccess> CaseAccesses { get; set; } = new List<CaseAccess>();
    public virtual ICollection<DocumentRequest> DocumentRequests { get; set; } = new List<DocumentRequest>();
    public virtual ICollection<HardCopyRequest> HardCopyRequests { get; set; } = new List<HardCopyRequest>();
}
