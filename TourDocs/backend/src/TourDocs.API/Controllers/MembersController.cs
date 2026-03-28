using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourDocs.Core.DTOs.Common;
using TourDocs.Core.DTOs.Members;
using TourDocs.Core.Interfaces;

namespace TourDocs.API.Controllers;

/// <summary>
/// Manages members (people whose documents are tracked) within an organization.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Policy = "CanManageMembers")]
public class MembersController : ControllerBase
{
    private readonly IMemberService _memberService;
    private readonly ICurrentUserService _currentUserService;

    public MembersController(IMemberService memberService, ICurrentUserService currentUserService)
    {
        _memberService = memberService;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Gets a member by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<MemberResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _memberService.GetByIdAsync(id);
        return Ok(ApiResponse<MemberResponse>.SuccessResult(result));
    }

    /// <summary>
    /// Gets all members for the current organization with pagination.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<MemberResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] PagedRequest pagedRequest)
    {
        var orgId = _currentUserService.OrganizationId!.Value;
        var result = await _memberService.GetByOrganizationAsync(orgId, pagedRequest);
        return Ok(ApiResponse<PagedResponse<MemberResponse>>.SuccessResult(result));
    }

    /// <summary>
    /// Searches members by name, email, passport number, or other fields.
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<MemberResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] PagedRequest pagedRequest)
    {
        var result = await _memberService.SearchAsync(query, pagedRequest);
        return Ok(ApiResponse<PagedResponse<MemberResponse>>.SuccessResult(result));
    }

    /// <summary>
    /// Creates a new member in the current organization.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<MemberResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMemberRequest request)
    {
        var result = await _memberService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponse<MemberResponse>.SuccessResult(result, "Member created."));
    }

    /// <summary>
    /// Updates an existing member.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<MemberResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMemberRequest request)
    {
        var result = await _memberService.UpdateAsync(id, request);
        return Ok(ApiResponse<MemberResponse>.SuccessResult(result, "Member updated."));
    }

    /// <summary>
    /// Soft-deletes a member.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _memberService.DeleteAsync(id);
        return NoContent();
    }
}
