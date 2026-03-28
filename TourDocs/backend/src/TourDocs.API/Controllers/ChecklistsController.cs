using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourDocs.Core.DTOs.Checklists;
using TourDocs.Core.DTOs.Common;
using TourDocs.Core.Interfaces;

namespace TourDocs.API.Controllers;

/// <summary>
/// Manages document checklists and their items.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ChecklistsController : ControllerBase
{
    private readonly IChecklistService _checklistService;
    private readonly ICurrentUserService _currentUserService;

    public ChecklistsController(IChecklistService checklistService, ICurrentUserService currentUserService)
    {
        _checklistService = checklistService;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Gets all checklists for the current organization.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ChecklistResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var orgId = _currentUserService.OrganizationId!.Value;
        var result = await _checklistService.GetByOrganizationAsync(orgId);
        return Ok(ApiResponse<IReadOnlyList<ChecklistResponse>>.SuccessResult(result));
    }

    /// <summary>
    /// Gets a checklist by ID with all its items.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ChecklistResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _checklistService.GetByIdAsync(id);
        return Ok(ApiResponse<ChecklistResponse>.SuccessResult(result));
    }

    /// <summary>
    /// Gets checklists by country code.
    /// </summary>
    [HttpGet("by-country/{countryCode}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ChecklistResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCountry(string countryCode)
    {
        var result = await _checklistService.GetByCountryAsync(countryCode);
        return Ok(ApiResponse<IReadOnlyList<ChecklistResponse>>.SuccessResult(result));
    }

    /// <summary>
    /// Gets all checklists for the current organization.
    /// </summary>
    [HttpGet("organization/{organizationId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ChecklistResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByOrganization(Guid organizationId)
    {
        var result = await _checklistService.GetByOrganizationAsync(organizationId);
        return Ok(ApiResponse<IReadOnlyList<ChecklistResponse>>.SuccessResult(result));
    }

    /// <summary>
    /// Gets all system-defined checklists.
    /// </summary>
    [HttpGet("system")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ChecklistResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSystemChecklists()
    {
        var result = await _checklistService.GetSystemChecklistsAsync();
        return Ok(ApiResponse<IReadOnlyList<ChecklistResponse>>.SuccessResult(result));
    }

    /// <summary>
    /// Creates a new checklist with items.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "CanManageCases")]
    [ProducesResponseType(typeof(ApiResponse<ChecklistResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateChecklistRequest request)
    {
        var result = await _checklistService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponse<ChecklistResponse>.SuccessResult(result, "Checklist created."));
    }

    /// <summary>
    /// Updates an existing checklist.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "CanManageCases")]
    [ProducesResponseType(typeof(ApiResponse<ChecklistResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateChecklistRequest request)
    {
        var result = await _checklistService.UpdateAsync(id, request);
        return Ok(ApiResponse<ChecklistResponse>.SuccessResult(result, "Checklist updated."));
    }

    /// <summary>
    /// Deletes a checklist (deactivates it).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "CanManageCases")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _checklistService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Adds an item to a checklist.
    /// </summary>
    [HttpPost("{id:guid}/items")]
    [Authorize(Policy = "CanManageCases")]
    [ProducesResponseType(typeof(ApiResponse<ChecklistItemResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> AddItem(Guid id, [FromBody] CreateChecklistItemRequest request)
    {
        var result = await _checklistService.AddItemAsync(id, request);
        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<ChecklistItemResponse>.SuccessResult(result, "Item added."));
    }

    /// <summary>
    /// Removes an item from a checklist.
    /// </summary>
    [HttpDelete("{id:guid}/items/{itemId:guid}")]
    [Authorize(Policy = "CanManageCases")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveItem(Guid id, Guid itemId)
    {
        await _checklistService.RemoveItemAsync(id, itemId);
        return NoContent();
    }
}
