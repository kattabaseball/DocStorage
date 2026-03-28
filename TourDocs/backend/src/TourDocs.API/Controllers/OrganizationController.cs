using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourDocs.Core.DTOs.Common;
using TourDocs.Core.DTOs.Organizations;
using TourDocs.Core.Interfaces;

namespace TourDocs.API.Controllers;

/// <summary>
/// Handles organization-scoped endpoints that use the current user's organization context.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class OrganizationController : ControllerBase
{
    private readonly IOrganizationService _organizationService;
    private readonly ICurrentUserService _currentUser;

    public OrganizationController(
        IOrganizationService organizationService,
        ICurrentUserService currentUser)
    {
        _organizationService = organizationService;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Gets the current organization's settings.
    /// </summary>
    [HttpGet("settings")]
    [ProducesResponseType(typeof(ApiResponse<OrganizationSettingsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSettings()
    {
        var orgId = _currentUser.OrganizationId
            ?? throw new UnauthorizedAccessException("No organization context.");

        var result = await _organizationService.GetSettingsAsync(orgId);
        return Ok(ApiResponse<OrganizationSettingsResponse>.SuccessResult(result));
    }

    /// <summary>
    /// Updates the current organization's settings.
    /// </summary>
    [HttpPut("settings")]
    [ProducesResponseType(typeof(ApiResponse<OrganizationSettingsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateSettings([FromBody] UpdateOrganizationSettingsRequest request)
    {
        var orgId = _currentUser.OrganizationId
            ?? throw new UnauthorizedAccessException("No organization context.");

        var result = await _organizationService.UpdateSettingsAsync(orgId, request);
        return Ok(ApiResponse<OrganizationSettingsResponse>.SuccessResult(result, "Settings saved."));
    }

    /// <summary>
    /// Gets the current organization's team members.
    /// </summary>
    [HttpGet("team")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<OrganizationMemberResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTeam()
    {
        var orgId = _currentUser.OrganizationId
            ?? throw new UnauthorizedAccessException("No organization context.");

        var result = await _organizationService.GetMembersAsync(orgId);
        return Ok(ApiResponse<IReadOnlyList<OrganizationMemberResponse>>.SuccessResult(result));
    }

    /// <summary>
    /// Invites a user to the current organization.
    /// </summary>
    [HttpPost("invite")]
    [Authorize(Policy = "OrgOwnerOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> InviteMember([FromBody] InviteOrganizationMemberRequest request)
    {
        var orgId = _currentUser.OrganizationId
            ?? throw new UnauthorizedAccessException("No organization context.");

        await _organizationService.InviteMemberAsync(orgId, request);
        return NoContent();
    }
}
