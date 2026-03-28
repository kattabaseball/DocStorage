using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourDocs.Core.DTOs.Common;
using TourDocs.Core.DTOs.Dashboard;
using TourDocs.Core.Interfaces;

namespace TourDocs.API.Controllers;

/// <summary>
/// Provides dashboard summary data and analytics.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly ICurrentUserService _currentUserService;

    public DashboardController(IDashboardService dashboardService, ICurrentUserService currentUserService)
    {
        _dashboardService = dashboardService;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Gets the dashboard summary for the current organization.
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(ApiResponse<DashboardSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary()
    {
        var orgId = _currentUserService.OrganizationId!.Value;
        var result = await _dashboardService.GetSummaryAsync(orgId);
        return Ok(ApiResponse<DashboardSummaryResponse>.SuccessResult(result));
    }

    /// <summary>
    /// Gets documents expiring within the specified number of days.
    /// </summary>
    [HttpGet("expiring-documents")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ExpiringDocumentResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetExpiringDocuments([FromQuery] int daysAhead = 30)
    {
        var orgId = _currentUserService.OrganizationId!.Value;
        var result = await _dashboardService.GetExpiringDocumentsAsync(orgId, daysAhead);
        return Ok(ApiResponse<IReadOnlyList<ExpiringDocumentResponse>>.SuccessResult(result));
    }

    /// <summary>
    /// Gets case readiness percentages for the current organization.
    /// </summary>
    [HttpGet("case-readiness")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CaseReadinessResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCaseReadiness([FromQuery] int count = 5)
    {
        var orgId = _currentUserService.OrganizationId!.Value;
        var result = await _dashboardService.GetCaseReadinessAsync(orgId, count);
        return Ok(ApiResponse<IReadOnlyList<CaseReadinessResponse>>.SuccessResult(result));
    }

    /// <summary>
    /// Gets recent activity for the current organization.
    /// </summary>
    [HttpGet("recent-activity")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RecentActivityResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecentActivity([FromQuery] int count = 10)
    {
        var orgId = _currentUserService.OrganizationId!.Value;
        var result = await _dashboardService.GetRecentActivityAsync(orgId, count);
        return Ok(ApiResponse<IReadOnlyList<RecentActivityResponse>>.SuccessResult(result));
    }
}
