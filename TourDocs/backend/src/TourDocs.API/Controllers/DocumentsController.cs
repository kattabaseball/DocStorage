using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourDocs.Core.DTOs.Common;
using TourDocs.Core.DTOs.Documents;
using TourDocs.Core.Interfaces;
using TourDocs.Domain.Enums;

namespace TourDocs.API.Controllers;

/// <summary>
/// Manages document upload, download, versioning, and verification workflows.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documentService;
    private readonly ICurrentUserService _currentUserService;

    public DocumentsController(IDocumentService documentService, ICurrentUserService currentUserService)
    {
        _documentService = documentService;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Gets all documents for the current organization with pagination.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<DocumentResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] PagedRequest pagedRequest)
    {
        var orgId = _currentUserService.OrganizationId!.Value;
        var result = await _documentService.GetByOrganizationAsync(orgId, pagedRequest);
        return Ok(ApiResponse<PagedResponse<DocumentResponse>>.SuccessResult(result));
    }

    /// <summary>
    /// Gets a document by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<DocumentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _documentService.GetByIdAsync(id);
        return Ok(ApiResponse<DocumentResponse>.SuccessResult(result));
    }

    /// <summary>
    /// Uploads a new document for a member.
    /// </summary>
    [HttpPost("upload")]
    [ProducesResponseType(typeof(ApiResponse<DocumentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Upload([FromForm] Guid memberId, [FromForm] DocumentCategory category,
        [FromForm] string documentType, [FromForm] string title, [FromForm] DateTime? expiryDate,
        [FromForm] bool isHardCopyNeeded, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResponse<object>.Failure("File is required."));

        if (file.Length > 52_428_800)
            return BadRequest(ApiResponse<object>.Failure("File size cannot exceed 50MB."));

        var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            return BadRequest(ApiResponse<object>.Failure($"File type '{extension}' is not allowed."));

        var request = new UploadDocumentRequest
        {
            MemberId = memberId,
            Category = category,
            DocumentType = documentType,
            Title = title,
            ExpiryDate = expiryDate,
            IsHardCopyNeeded = isHardCopyNeeded,
            FileName = file.FileName,
            FileSize = file.Length,
            MimeType = file.ContentType,
            FileStream = file.OpenReadStream()
        };

        var result = await _documentService.UploadAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponse<DocumentResponse>.SuccessResult(result, "Document uploaded."));
    }

    /// <summary>
    /// Downloads a document file.
    /// </summary>
    [HttpGet("{id:guid}/download")]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Download(Guid id)
    {
        var document = await _documentService.GetByIdAsync(id);
        var stream = await _documentService.DownloadAsync(id);
        return File(stream, "application/octet-stream", $"{document.Title}");
    }

    /// <summary>
    /// Verifies a document, moving it to Verified status.
    /// </summary>
    [HttpPut("{id:guid}/verify")]
    [Authorize(Policy = "CanVerifyDocuments")]
    [ProducesResponseType(typeof(ApiResponse<DocumentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Verify(Guid id, [FromBody] string? notes = null)
    {
        var result = await _documentService.VerifyAsync(id, notes);
        return Ok(ApiResponse<DocumentResponse>.SuccessResult(result, "Document verified."));
    }

    /// <summary>
    /// Rejects a document with an optional reason.
    /// </summary>
    [HttpPut("{id:guid}/reject")]
    [Authorize(Policy = "CanVerifyDocuments")]
    [ProducesResponseType(typeof(ApiResponse<DocumentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reject(Guid id, [FromBody] string? reason = null)
    {
        var result = await _documentService.RejectAsync(id, reason);
        return Ok(ApiResponse<DocumentResponse>.SuccessResult(result, "Document rejected."));
    }

    /// <summary>
    /// Re-uploads a new version of an existing document.
    /// </summary>
    [HttpPost("{id:guid}/reupload")]
    [ProducesResponseType(typeof(ApiResponse<DocumentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reupload(Guid id, IFormFile file)
    {
        var doc = await _documentService.GetByIdAsync(id);
        var request = new UploadDocumentRequest
        {
            MemberId = doc.MemberId,
            Category = doc.Category,
            DocumentType = doc.DocumentType,
            Title = doc.Title,
            ExpiryDate = doc.ExpiryDate,
            IsHardCopyNeeded = doc.IsHardCopyNeeded,
            FileName = file.FileName,
            FileSize = file.Length,
            MimeType = file.ContentType,
            FileStream = file.OpenReadStream()
        };

        var result = await _documentService.ReuploadAsync(id, request);
        return Ok(ApiResponse<DocumentResponse>.SuccessResult(result, "Document version uploaded."));
    }

    /// <summary>
    /// Deletes a document (soft delete).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _documentService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Gets all documents for a specific member.
    /// </summary>
    [HttpGet("by-member/{memberId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<DocumentResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByMember(Guid memberId)
    {
        var result = await _documentService.GetByMemberAsync(memberId);
        return Ok(ApiResponse<IReadOnlyList<DocumentResponse>>.SuccessResult(result));
    }

    /// <summary>
    /// Gets documents that are expiring within the specified number of days.
    /// </summary>
    [HttpGet("expiring")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<DocumentResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetExpiring([FromQuery] int daysThreshold = 30)
    {
        var result = await _documentService.GetExpiringDocumentsAsync(daysThreshold);
        return Ok(ApiResponse<IReadOnlyList<DocumentResponse>>.SuccessResult(result));
    }
}
