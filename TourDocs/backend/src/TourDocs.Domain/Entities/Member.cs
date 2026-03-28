using TourDocs.Domain.Common;

namespace TourDocs.Domain.Entities;

/// <summary>
/// Represents a member (e.g., traveler, employee) whose documents are managed by an organization.
/// </summary>
public class Member : AuditableEntity, ISoftDelete
{
    public Guid OrganizationId { get; set; }
    public Guid? UserId { get; set; }
    public string LegalFirstName { get; set; } = string.Empty;
    public string LegalLastName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? Nationality { get; set; }
    public string? NicNumber { get; set; }
    public string? PassportNumber { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? Title { get; set; }
    public string? Department { get; set; }
    public string? Specialization { get; set; }
    public string? ExternalId { get; set; }
    public string? CustomFields { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public virtual Organization Organization { get; set; } = null!;
    public virtual ApplicationUser? User { get; set; }
    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
    public virtual ICollection<TravelHistory> TravelHistory { get; set; } = new List<TravelHistory>();
    public virtual ICollection<CaseMember> CaseMembers { get; set; } = new List<CaseMember>();
}
