namespace TourDocs.Core.DTOs.Members;

/// <summary>
/// Request DTO for creating a new member.
/// </summary>
public class CreateMemberRequest
{
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
    public string? Notes { get; set; }
}
