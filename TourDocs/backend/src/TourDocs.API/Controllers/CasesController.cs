using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourDocs.Core.DTOs.Cases;
using TourDocs.Core.DTOs.Common;
using TourDocs.Core.Interfaces;

namespace TourDocs.API.Controllers;

/// <summary>
/// Manages cases (document collection scenarios) and their lifecycle.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Policy = "CanManageCases")]
public class CasesController : ControllerBase
{
    private readonly ICaseService _caseService;
    private readonly ICurrentUserService _currentUserService;

    public CasesController(ICaseService caseService, ICurrentUserService currentUserService)
    {
        _caseService = caseService;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Gets all cases for the current organization with pagination.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<CaseResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] PagedRequest pagedRequest)
    {
        var orgId = _currentUserService.OrganizationId!.Value;
        var result = await _caseService.GetByOrganizationAsync(orgId, pagedRequest);
        return Ok(ApiResponse<PagedResponse<CaseResponse>>.SuccessResult(result));
    }

    /// <summary>
    /// Gets a case by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CaseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _caseService.GetByIdAsync(id);
        return Ok(ApiResponse<CaseResponse>.SuccessResult(result));
    }

    /// <summary>
    /// Creates a new case.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CaseResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCaseRequest request)
    {
        var result = await _caseService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponse<CaseResponse>.SuccessResult(result, "Case created."));
    }

    /// <summary>
    /// Assigns members to a case.
    /// </summary>
    [HttpPost("{id:guid}/members")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AssignMembers(Guid id, [FromBody] List<Guid> memberIds)
    {
        await _caseService.AssignMembersAsync(id, memberIds);
        return NoContent();
    }

    /// <summary>
    /// Gets the document readiness percentage for a case.
    /// </summary>
    [HttpGet("{id:guid}/readiness")]
    [ProducesResponseType(typeof(ApiResponse<double>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReadiness(Guid id)
    {
        var result = await _caseService.GetReadinessPercentageAsync(id);
        return Ok(ApiResponse<double>.SuccessResult(result));
    }

    /// <summary>
    /// Grants access to a case for an external user.
    /// </summary>
    [HttpPost("{id:guid}/access")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GrantAccess(Guid id, [FromBody] GrantAccessRequest request)
    {
        await _caseService.GrantAccessAsync(id, request);
        return NoContent();
    }

    /// <summary>
    /// Revokes access to a case.
    /// </summary>
    [HttpDelete("access/{accessId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RevokeAccess(Guid accessId)
    {
        await _caseService.RevokeAccessAsync(accessId);
        return NoContent();
    }
}
