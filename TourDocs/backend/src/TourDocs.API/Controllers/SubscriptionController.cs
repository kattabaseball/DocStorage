using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourDocs.Core.DTOs.Common;
using TourDocs.Core.DTOs.Subscriptions;
using TourDocs.Core.Interfaces;

namespace TourDocs.API.Controllers;

/// <summary>
/// Handles subscription management for the current user's organization.
/// </summary>
[ApiController]
[Route("api/v1/subscription")]
[Authorize]
public class SubscriptionController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly ICurrentUserService _currentUser;

    public SubscriptionController(
        ISubscriptionService subscriptionService,
        ICurrentUserService currentUser)
    {
        _subscriptionService = subscriptionService;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Gets the current organization's subscription details.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<SubscriptionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubscription()
    {
        var orgId = _currentUser.OrganizationId
            ?? throw new UnauthorizedAccessException("No organization context.");

        var result = await _subscriptionService.GetByOrganizationAsync(orgId);
        return Ok(ApiResponse<SubscriptionResponse>.SuccessResult(result));
    }

    /// <summary>
    /// Gets the current organization's subscription usage statistics.
    /// </summary>
    [HttpGet("usage")]
    [ProducesResponseType(typeof(ApiResponse<SubscriptionUsageResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsage()
    {
        var orgId = _currentUser.OrganizationId
            ?? throw new UnauthorizedAccessException("No organization context.");

        var result = await _subscriptionService.GetUsageAsync(orgId);
        return Ok(ApiResponse<SubscriptionUsageResponse>.SuccessResult(result));
    }

    /// <summary>
    /// Updates the current organization's subscription plan. Requires organization owner role.
    /// </summary>
    [HttpPut]
    [Authorize(Policy = "OrgOwnerOnly")]
    [ProducesResponseType(typeof(ApiResponse<SubscriptionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePlan([FromBody] UpdateSubscriptionRequest request)
    {
        var orgId = _currentUser.OrganizationId
            ?? throw new UnauthorizedAccessException("No organization context.");

        var result = await _subscriptionService.UpdatePlanAsync(orgId, request);
        return Ok(ApiResponse<SubscriptionResponse>.SuccessResult(result, "Subscription plan updated."));
    }
}
