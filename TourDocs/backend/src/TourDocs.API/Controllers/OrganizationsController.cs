using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourDocs.Core.DTOs.Common;
using TourDocs.Core.DTOs.Organizations;
using TourDocs.Core.Interfaces;

namespace TourDocs.API.Controllers;

/// <summary>
/// Manages organization settings and team members.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class OrganizationsController : ControllerBase
{
    private readonly IOrganizationService _organizationService;

    public OrganizationsController(IOrganizationService organizationService)
    {
        _organizationService = organizationService;
    }

    /// <summary>
    /// Gets an organization by its unique identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<OrganizationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _organizationService.GetByIdAsync(id);
        return Ok(ApiResponse<OrganizationResponse>.SuccessResult(result));
    }

    /// <summary>
    /// Gets an organization by its URL slug.
    /// </summary>
    [HttpGet("by-slug/{slug}")]
    [ProducesResponseType(typeof(ApiResponse<OrganizationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var result = await _organizationService.GetBySlugAsync(slug);
        return Ok(ApiResponse<OrganizationResponse>.SuccessResult(result));
    }

    /// <summary>
    /// Gets detailed organization information including member counts and subscription.
    /// </summary>
    [HttpGet("{id:guid}/detail")]
    [ProducesResponseType(typeof(ApiResponse<OrganizationDetailResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDetail(Guid id)
    {
        var result = await _organizationService.GetDetailAsync(id);
        return Ok(ApiResponse<OrganizationDetailResponse>.SuccessResult(result));
    }

    /// <summary>
    /// Creates a new organization.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<OrganizationResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateOrganizationRequest request)
    {
        var result = await _organizationService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponse<OrganizationResponse>.SuccessResult(result, "Organization created."));
    }

    /// <summary>
    /// Updates an existing organization.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "OrgOwnerOnly")]
    [ProducesResponseType(typeof(ApiResponse<OrganizationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrganizationRequest request)
    {
        var result = await _organizationService.UpdateAsync(id, request);
        return Ok(ApiResponse<OrganizationResponse>.SuccessResult(result, "Organization updated."));
    }

    /// <summary>
    /// Gets all team members of an organization.
    /// </summary>
    [HttpGet("{id:guid}/members")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<OrganizationMemberResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMembers(Guid id)
    {
        var result = await _organizationService.GetMembersAsync(id);
        return Ok(ApiResponse<IReadOnlyList<OrganizationMemberResponse>>.SuccessResult(result));
    }

    /// <summary>
    /// Invites a user to join the organization.
    /// </summary>
    [HttpPost("{id:guid}/members/invite")]
    [Authorize(Policy = "OrgOwnerOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> InviteMember(Guid id, [FromBody] InviteOrganizationMemberRequest request)
    {
        await _organizationService.InviteMemberAsync(id, request);
        return NoContent();
    }

    /// <summary>
    /// Removes a user from the organization.
    /// </summary>
    [HttpDelete("{id:guid}/members/{userId:guid}")]
    [Authorize(Policy = "OrgOwnerOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveMember(Guid id, Guid userId)
    {
        await _organizationService.RemoveMemberAsync(id, userId);
        return NoContent();
    }
}
