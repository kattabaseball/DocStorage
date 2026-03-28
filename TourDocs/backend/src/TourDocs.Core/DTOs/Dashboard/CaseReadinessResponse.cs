namespace TourDocs.Core.DTOs.Dashboard;

/// <summary>
/// Response DTO for case readiness percentage data.
/// </summary>
public class CaseReadinessResponse
{
    public Guid CaseId { get; set; }
    public string CaseName { get; set; } = string.Empty;
    public int ReadyPercent { get; set; }
    public int PendingPercent { get; set; }
}
