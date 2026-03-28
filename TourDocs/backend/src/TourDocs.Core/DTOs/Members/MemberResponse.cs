namespace TourDocs.Core.DTOs.Members;

/// <summary>
/// Response DTO for a single member.
/// </summary>
public class MemberResponse
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid? UserId { get; set; }
    public string LegalFirstName { get; set; } = string.Empty;
    public string LegalLastName { get; set; } = string.Empty;
    public string FullName => $"{LegalFirstName} {LegalLastName}";
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
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int DocumentCount { get; set; }
}
