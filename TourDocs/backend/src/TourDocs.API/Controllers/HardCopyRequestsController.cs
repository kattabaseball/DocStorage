using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourDocs.Core.DTOs.Common;
using TourDocs.Core.DTOs.HardCopy;
using TourDocs.Core.Interfaces;

namespace TourDocs.API.Controllers;

/// <summary>
/// Manages hard copy document requests and chain-of-custody handovers.
/// </summary>
[ApiController]
[Route("api/v1/hard-copies")]
[Authorize]
public class HardCopyRequestsController : ControllerBase
{
    private readonly IHardCopyService _hardCopyService;
    private readonly ICurrentUserService _currentUserService;

    public HardCopyRequestsController(IHardCopyService hardCopyService, ICurrentUserService currentUserService)
    {
        _hardCopyService = hardCopyService;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Gets all hard copy requests for the current organization.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<HardCopyRequestResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var orgId = _currentUserService.OrganizationId!.Value;
        var result = await _hardCopyService.GetByOrganizationAsync(orgId);
        return Ok(ApiResponse<IReadOnlyList<HardCopyRequestResponse>>.SuccessResult(result));
    }

    /// <summary>
    /// Gets a hard copy request by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<HardCopyRequestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _hardCopyService.GetByIdAsync(id);
        return Ok(ApiResponse<HardCopyRequestResponse>.SuccessResult(result));
    }

    /// <summary>
    /// Gets all hard copy requests for a case.
    /// </summary>
    [HttpGet("by-case/{caseId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<HardCopyRequestResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCase(Guid caseId)
    {
        var result = await _hardCopyService.GetByCaseAsync(caseId);
        return Ok(ApiResponse<IReadOnlyList<HardCopyRequestResponse>>.SuccessResult(result));
    }

    /// <summary>
    /// Creates a new hard copy request.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<HardCopyRequestResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateHardCopyRequestDto request)
    {
        var result = await _hardCopyService.CreateRequestAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponse<HardCopyRequestResponse>.SuccessResult(result, "Hard copy request created."));
    }

    /// <summary>
    /// Updates the status of a hard copy request.
    /// </summary>
    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse<HardCopyRequestResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateHardCopyStatusRequest request)
    {
        var result = await _hardCopyService.UpdateStatusAsync(id, request);
        return Ok(ApiResponse<HardCopyRequestResponse>.SuccessResult(result, "Status updated."));
    }

    /// <summary>
    /// Records a handover event for a hard copy request.
    /// </summary>
    [HttpPost("{id:guid}/handovers")]
    [ProducesResponseType(typeof(ApiResponse<HardCopyHandoverResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> RecordHandover(Guid id, [FromBody] CreateHardCopyHandoverRequest request)
    {
        var result = await _hardCopyService.RecordHandoverAsync(id, request);
        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<HardCopyHandoverResponse>.SuccessResult(result, "Handover recorded."));
    }

    /// <summary>
    /// Gets the handover history for a hard copy request.
    /// </summary>
    [HttpGet("{id:guid}/handovers")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<HardCopyHandoverResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHandoverHistory(Guid id)
    {
        var result = await _hardCopyService.GetHandoverHistoryAsync(id);
        return Ok(ApiResponse<IReadOnlyList<HardCopyHandoverResponse>>.SuccessResult(result));
    }
}
