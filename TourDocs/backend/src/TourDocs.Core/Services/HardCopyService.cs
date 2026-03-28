using AutoMapper;
using Microsoft.Extensions.Logging;
using TourDocs.Core.DTOs.HardCopy;
using TourDocs.Core.Exceptions;
using TourDocs.Core.Interfaces;
using TourDocs.Domain.Entities;
using TourDocs.Domain.Interfaces;
using TourDocs.Domain.Enums;

namespace TourDocs.Core.Services;

/// <summary>
/// Business logic service for managing hard copy document chain-of-custody.
/// </summary>
public class HardCopyService : IHardCopyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ITenantContext _tenantContext;
    private readonly IAuditService _auditService;
    private readonly ILogger<HardCopyService> _logger;

    public HardCopyService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ITenantContext tenantContext,
        IAuditService auditService,
        ILogger<HardCopyService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _tenantContext = tenantContext;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<IReadOnlyList<HardCopyRequestResponse>> GetByOrganizationAsync(Guid organizationId)
    {
        var requests = await _unitOfWork.HardCopyRequests.FindAsync(
            h => h.Case != null && h.Case.OrganizationId == organizationId);
        return _mapper.Map<IReadOnlyList<HardCopyRequestResponse>>(requests.OrderByDescending(r => r.CreatedAt).ToList());
    }

    public async Task<HardCopyRequestResponse> GetByIdAsync(Guid id)
    {
        var request = await _unitOfWork.HardCopyRequests.GetByIdAsync(id)
            ?? throw new NotFoundException("HardCopyRequest", id);

        return _mapper.Map<HardCopyRequestResponse>(request);
    }

    public async Task<IReadOnlyList<HardCopyRequestResponse>> GetByCaseAsync(Guid caseId)
    {
        var requests = await _unitOfWork.HardCopyRequests.FindAsync(r => r.CaseId == caseId);
        return _mapper.Map<IReadOnlyList<HardCopyRequestResponse>>(requests);
    }

    public async Task<HardCopyRequestResponse> CreateRequestAsync(CreateHardCopyRequestDto request)
    {
        var document = await _unitOfWork.Documents.GetByIdAsync(request.DocumentId)
            ?? throw new NotFoundException("Document", request.DocumentId);

        var caseEntity = await _unitOfWork.Cases.GetByIdAsync(request.CaseId)
            ?? throw new NotFoundException("Case", request.CaseId);

        var hardCopyRequest = new HardCopyRequest
        {
            Id = Guid.NewGuid(),
            DocumentId = request.DocumentId,
            CaseId = request.CaseId,
            RequestedBy = _tenantContext.UserId,
            Status = HardCopyStatus.Requested,
            Urgency = request.Urgency,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.HardCopyRequests.AddAsync(hardCopyRequest);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogAsync(
            _tenantContext.OrganizationId, _tenantContext.UserId,
            "HardCopy.Requested", "HardCopyRequest", hardCopyRequest.Id,
            $"Hard copy requested for document: {document.Title}");

        _logger.LogInformation("Hard copy request {RequestId} created for document {DocId}",
            hardCopyRequest.Id, request.DocumentId);

        return _mapper.Map<HardCopyRequestResponse>(hardCopyRequest);
    }

    public async Task<HardCopyRequestResponse> UpdateStatusAsync(Guid id, UpdateHardCopyStatusRequest request)
    {
        var hardCopyRequest = await _unitOfWork.HardCopyRequests.GetByIdAsync(id)
            ?? throw new NotFoundException("HardCopyRequest", id);

        ValidateStatusTransition(hardCopyRequest.Status, request.Status);

        hardCopyRequest.Status = request.Status;
        hardCopyRequest.Notes = request.Notes ?? hardCopyRequest.Notes;
        hardCopyRequest.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.HardCopyRequests.Update(hardCopyRequest);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogAsync(
            _tenantContext.OrganizationId, _tenantContext.UserId,
            "HardCopy.StatusChanged", "HardCopyRequest", id,
            $"Status changed to: {request.Status}");

        return _mapper.Map<HardCopyRequestResponse>(hardCopyRequest);
    }

    public async Task<HardCopyHandoverResponse> RecordHandoverAsync(Guid hardCopyRequestId, CreateHardCopyHandoverRequest request)
    {
        var hardCopyRequest = await _unitOfWork.HardCopyRequests.GetByIdAsync(hardCopyRequestId)
            ?? throw new NotFoundException("HardCopyRequest", hardCopyRequestId);

        var handover = new HardCopyHandover
        {
            Id = Guid.NewGuid(),
            HardCopyRequestId = hardCopyRequestId,
            FromUserId = _tenantContext.UserId,
            ToUserId = request.ToUserId,
            FromRole = "Current",
            ToRole = "Recipient",
            HandoverType = request.HandoverType,
            ConfirmationMethod = request.ConfirmationMethod,
            ConfirmationData = request.ConfirmationData,
            Notes = request.Notes,
            RecordedAt = DateTime.UtcNow,
            RecordedBy = _tenantContext.UserId
        };

        await _unitOfWork.HardCopyHandovers.AddAsync(handover);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Handover {HandoverId} recorded for hard copy request {RequestId}",
            handover.Id, hardCopyRequestId);

        return _mapper.Map<HardCopyHandoverResponse>(handover);
    }

    public async Task<IReadOnlyList<HardCopyHandoverResponse>> GetHandoverHistoryAsync(Guid hardCopyRequestId)
    {
        var handovers = await _unitOfWork.HardCopyHandovers.FindAsync(
            h => h.HardCopyRequestId == hardCopyRequestId);

        return _mapper.Map<IReadOnlyList<HardCopyHandoverResponse>>(
            handovers.OrderBy(h => h.RecordedAt).ToList());
    }

    private static void ValidateStatusTransition(HardCopyStatus current, HardCopyStatus next)
    {
        var validTransitions = new Dictionary<HardCopyStatus, HardCopyStatus[]>
        {
            { HardCopyStatus.WithMember, new[] { HardCopyStatus.Requested } },
            { HardCopyStatus.Requested, new[] { HardCopyStatus.Acknowledged } },
            { HardCopyStatus.Acknowledged, new[] { HardCopyStatus.CollectedByManager } },
            { HardCopyStatus.CollectedByManager, new[] { HardCopyStatus.HandedToHandler } },
            { HardCopyStatus.HandedToHandler, new[] { HardCopyStatus.AtAuthority } },
            { HardCopyStatus.AtAuthority, new[] { HardCopyStatus.ReturnedToManager } },
            { HardCopyStatus.ReturnedToManager, new[] { HardCopyStatus.ReturnedToMember } }
        };

        if (!validTransitions.TryGetValue(current, out var allowed) || !allowed.Contains(next))
        {
            throw new BusinessRuleException(
                $"Invalid status transition from '{current}' to '{next}'.");
        }
    }
}
