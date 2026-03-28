using AutoMapper;
using Microsoft.Extensions.Logging;
using TourDocs.Core.DTOs.DocumentRequests;
using TourDocs.Core.Exceptions;
using TourDocs.Core.Interfaces;
using TourDocs.Domain.Entities;
using TourDocs.Domain.Interfaces;
using TourDocs.Domain.Enums;

namespace TourDocs.Core.Services;

/// <summary>
/// Business logic service for managing document requests.
/// </summary>
public class DocumentRequestService : IDocumentRequestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ITenantContext _tenantContext;
    private readonly IAuditService _auditService;
    private readonly ILogger<DocumentRequestService> _logger;

    public DocumentRequestService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ITenantContext tenantContext,
        IAuditService auditService,
        ILogger<DocumentRequestService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _tenantContext = tenantContext;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<DocumentRequestResponse> GetByIdAsync(Guid id)
    {
        var request = await _unitOfWork.DocumentRequests.GetByIdAsync(id)
            ?? throw new NotFoundException("DocumentRequest", id);

        return _mapper.Map<DocumentRequestResponse>(request);
    }

    public async Task<IReadOnlyList<DocumentRequestResponse>> GetByMemberAsync(Guid memberId)
    {
        var requests = await _unitOfWork.DocumentRequests.FindAsync(r => r.MemberId == memberId);
        return _mapper.Map<IReadOnlyList<DocumentRequestResponse>>(requests.OrderByDescending(r => r.CreatedAt).ToList());
    }

    public async Task<IReadOnlyList<DocumentRequestResponse>> GetByCaseAsync(Guid caseId)
    {
        var requests = await _unitOfWork.DocumentRequests.FindAsync(r => r.CaseId == caseId);
        return _mapper.Map<IReadOnlyList<DocumentRequestResponse>>(requests.OrderByDescending(r => r.CreatedAt).ToList());
    }

    public async Task<DocumentRequestResponse> CreateAsync(CreateDocumentRequestDto request)
    {
        var member = await _unitOfWork.Members.GetByIdAsync(request.MemberId)
            ?? throw new NotFoundException("Member", request.MemberId);

        if (request.CaseId.HasValue)
        {
            var caseEntity = await _unitOfWork.Cases.GetByIdAsync(request.CaseId.Value)
                ?? throw new NotFoundException("Case", request.CaseId.Value);
        }

        var docRequest = new DocumentRequest
        {
            Id = Guid.NewGuid(),
            CaseId = request.CaseId,
            MemberId = request.MemberId,
            RequestedBy = _tenantContext.UserId,
            DocumentType = request.DocumentType,
            FormatRequirements = request.FormatRequirements,
            Urgency = request.Urgency,
            Notes = request.Notes,
            Status = DocumentRequestStatus.Requested,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.DocumentRequests.AddAsync(docRequest);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Document request {RequestId} created for member {MemberId}",
            docRequest.Id, request.MemberId);

        return _mapper.Map<DocumentRequestResponse>(docRequest);
    }

    public async Task<DocumentRequestResponse> UpdateStatusAsync(Guid id, UpdateDocumentRequestStatusDto request)
    {
        var docRequest = await _unitOfWork.DocumentRequests.GetByIdAsync(id)
            ?? throw new NotFoundException("DocumentRequest", id);

        docRequest.Status = request.Status;
        docRequest.DeclineReason = request.DeclineReason;
        docRequest.FulfilledDocumentId = request.FulfilledDocumentId;
        docRequest.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.DocumentRequests.Update(docRequest);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<DocumentRequestResponse>(docRequest);
    }

    public async Task<DocumentRequestResponse> FulfillAsync(Guid id, Guid documentId)
    {
        var docRequest = await _unitOfWork.DocumentRequests.GetByIdAsync(id)
            ?? throw new NotFoundException("DocumentRequest", id);

        var document = await _unitOfWork.Documents.GetByIdAsync(documentId)
            ?? throw new NotFoundException("Document", documentId);

        docRequest.Status = DocumentRequestStatus.Fulfilled;
        docRequest.FulfilledDocumentId = documentId;
        docRequest.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.DocumentRequests.Update(docRequest);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogAsync(
            _tenantContext.OrganizationId, _tenantContext.UserId,
            "DocumentRequest.Fulfilled", "DocumentRequest", id,
            $"Fulfilled with document: {document.Title}");

        return _mapper.Map<DocumentRequestResponse>(docRequest);
    }

    public async Task<DocumentRequestResponse> DeclineAsync(Guid id, string reason)
    {
        var docRequest = await _unitOfWork.DocumentRequests.GetByIdAsync(id)
            ?? throw new NotFoundException("DocumentRequest", id);

        docRequest.Status = DocumentRequestStatus.Declined;
        docRequest.DeclineReason = reason;
        docRequest.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.DocumentRequests.Update(docRequest);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<DocumentRequestResponse>(docRequest);
    }
}
