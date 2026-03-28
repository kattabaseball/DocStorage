using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourDocs.Core.DTOs.Audit;
using TourDocs.Core.DTOs.Common;
using TourDocs.Core.Interfaces;
using TourDocs.Domain.Interfaces;

namespace TourDocs.API.Controllers;

/// <summary>
/// Provides access to audit log data for compliance and monitoring.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Policy = "CanViewAudit")]
public class AuditController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public AuditController(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Gets audit logs for the current organization with pagination.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AuditLogResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var orgId = _currentUserService.OrganizationId!.Value;
        var logs = await _unitOfWork.AuditLogs.FindAsync(a => a.OrganizationId == orgId);

        var paged = logs
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var result = _mapper.Map<IReadOnlyList<AuditLogResponse>>(paged);
        return Ok(ApiResponse<IReadOnlyList<AuditLogResponse>>.SuccessResult(result));
    }

    /// <summary>
    /// Gets audit logs for a specific entity.
    /// </summary>
    [HttpGet("entity/{entityType}/{entityId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AuditLogResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByEntity(string entityType, Guid entityId)
    {
        var orgId = _currentUserService.OrganizationId!.Value;
        var logs = await _unitOfWork.AuditLogs.FindAsync(
            a => a.OrganizationId == orgId && a.EntityType == entityType && a.EntityId == entityId);

        var result = _mapper.Map<IReadOnlyList<AuditLogResponse>>(
            logs.OrderByDescending(a => a.CreatedAt).ToList());
        return Ok(ApiResponse<IReadOnlyList<AuditLogResponse>>.SuccessResult(result));
    }

    /// <summary>
    /// Gets audit logs for a specific user.
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AuditLogResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByUser(Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var orgId = _currentUserService.OrganizationId!.Value;
        var logs = await _unitOfWork.AuditLogs.FindAsync(a => a.OrganizationId == orgId && a.UserId == userId);

        var paged = logs
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var result = _mapper.Map<IReadOnlyList<AuditLogResponse>>(paged);
        return Ok(ApiResponse<IReadOnlyList<AuditLogResponse>>.SuccessResult(result));
    }
}
