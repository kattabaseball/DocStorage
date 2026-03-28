namespace TourDocs.Core.DTOs.Dashboard;

/// <summary>
/// Response DTO containing dashboard summary statistics.
/// </summary>
public class DashboardSummaryResponse
{
    public int TotalMembers { get; set; }
    public int ActiveCases { get; set; }
    public int TotalDocuments { get; set; }
    public int PendingVerifications { get; set; }
    public int ExpiringDocuments { get; set; }
    public int PendingHardCopyRequests { get; set; }
    public int PendingDocumentRequests { get; set; }
    public int UnreadNotifications { get; set; }

    // Document status breakdown for the health chart
    public int VerifiedDocuments { get; set; }
    public int UploadedDocuments { get; set; }
    public int RejectedDocuments { get; set; }
    public int ExpiredDocuments { get; set; }
}
