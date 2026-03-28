using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourDocs.Core.DTOs.Common;
using TourDocs.Core.DTOs.DocumentRequests;
using TourDocs.Core.Interfaces;

namespace TourDocs.API.Controllers;

/// <summary>
/// Manages document requests from organizations to members.
/// </summary>
[ApiController]
[Route("api/v1/document-requests")]
[Authorize]
public class DocumentRequestsController : ControllerBase
{
    private readonly IDocumentRequestService _documentRequestService;

    public DocumentRequestsController(IDocumentRequestService documentRequestService)
    {
        _documentRequestService = documentRequestService;
    }

    /// <summary>
    /// Gets a document request by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<DocumentRequestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _documentRequestService.GetByIdAsync(id);
        return Ok(ApiResponse<DocumentRequestResponse>.SuccessResult(result));
    }

    /// <summary>
    /// Gets all document requests for a member.
    /// </summary>
    [HttpGet("by-member/{memberId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<DocumentRequestResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByMember(Guid memberId)
    {
        var result = await _documentRequestService.GetByMemberAsync(memberId);
        return Ok(ApiResponse<IReadOnlyList<DocumentRequestResponse>>.SuccessResult(result));
    }

    /// <summary>
    /// Gets all document requests for a case.
    /// </summary>
    [HttpGet("by-case/{caseId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<DocumentRequestResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCase(Guid caseId)
    {
        var result = await _documentRequestService.GetByCaseAsync(caseId);
        return Ok(ApiResponse<IReadOnlyList<DocumentRequestResponse>>.SuccessResult(result));
    }

    /// <summary>
    /// Creates a new document request.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<DocumentRequestResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateDocumentRequestDto request)
    {
        var result = await _documentRequestService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponse<DocumentRequestResponse>.SuccessResult(result, "Document request created."));
    }

    /// <summary>
    /// Updates the status of a document request.
    /// </summary>
    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse<DocumentRequestResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateDocumentRequestStatusDto request)
    {
        var result = await _documentRequestService.UpdateStatusAsync(id, request);
        return Ok(ApiResponse<DocumentRequestResponse>.SuccessResult(result, "Status updated."));
    }

    /// <summary>
    /// Fulfills a document request by linking it to an uploaded document.
    /// </summary>
    [HttpPut("{id:guid}/fulfill")]
    [ProducesResponseType(typeof(ApiResponse<DocumentRequestResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Fulfill(Guid id, [FromBody] Guid documentId)
    {
        var result = await _documentRequestService.FulfillAsync(id, documentId);
        return Ok(ApiResponse<DocumentRequestResponse>.SuccessResult(result, "Request fulfilled."));
    }

    /// <summary>
    /// Declines a document request with a reason.
    /// </summary>
    [HttpPut("{id:guid}/decline")]
    [ProducesResponseType(typeof(ApiResponse<DocumentRequestResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Decline(Guid id, [FromBody] string reason)
    {
        var result = await _documentRequestService.DeclineAsync(id, reason);
        return Ok(ApiResponse<DocumentRequestResponse>.SuccessResult(result, "Request declined."));
    }
}
