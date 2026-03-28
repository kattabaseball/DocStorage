using TourDocs.Core.DTOs.Dashboard;

namespace TourDocs.Core.Interfaces;

/// <summary>
/// Service contract for dashboard data aggregation.
/// </summary>
public interface IDashboardService
{
    Task<DashboardSummaryResponse> GetSummaryAsync(Guid organizationId);
    Task<IReadOnlyList<ExpiringDocumentResponse>> GetExpiringDocumentsAsync(Guid organizationId, int daysAhead = 30);
    Task<IReadOnlyList<RecentActivityResponse>> GetRecentActivityAsync(Guid organizationId, int count = 10);
    Task<IReadOnlyList<CaseReadinessResponse>> GetCaseReadinessAsync(Guid organizationId, int count = 5);
}
