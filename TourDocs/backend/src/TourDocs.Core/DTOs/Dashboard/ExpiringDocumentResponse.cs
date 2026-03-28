namespace TourDocs.Core.DTOs.Dashboard;

/// <summary>
/// Response DTO for a document approaching its expiry date.
/// </summary>
public class ExpiringDocumentResponse
{
    public Guid DocumentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
    public int DaysUntilExpiry { get; set; }
}
