using Microsoft.Extensions.Logging;
using TourDocs.Core.DTOs.Common;
using TourDocs.Core.DTOs.Documents;
using TourDocs.Core.Exceptions;
using TourDocs.Core.Interfaces;
using TourDocs.Domain.Entities;
using TourDocs.Domain.Interfaces;
using TourDocs.Domain.Enums;

namespace TourDocs.Core.Services;

/// <summary>
/// Business logic service for managing documents including upload, versioning, and verification.
/// </summary>
public class DocumentService : IDocumentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorage;
    private readonly ITenantContext _tenantContext;
    private readonly IAuditService _auditService;
    private readonly ILogger<DocumentService> _logger;

    public DocumentService(
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorage,
        ITenantContext tenantContext,
        IAuditService auditService,
        ILogger<DocumentService> logger)
    {
        _unitOfWork = unitOfWork;
        _fileStorage = fileStorage;
        _tenantContext = tenantContext;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<PagedResponse<DocumentResponse>> GetByOrganizationAsync(Guid organizationId, PagedRequest request)
    {
        var documents = await _unitOfWork.Documents.FindAsync(
            d => d.OrganizationId == organizationId);

        var query = documents.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLowerInvariant();
            query = query.Where(d =>
                d.Title.ToLower().Contains(term) ||
                d.DocumentType.ToLower().Contains(term));
        }

        var totalCount = query.Count();
        var pagedDocs = query
            .OrderByDescending(d => d.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // Load members for these documents
        var memberIds = pagedDocs.Select(d => d.MemberId).Distinct().ToList();
        var members = await _unitOfWork.Members.FindAsync(m => memberIds.Contains(m.Id));
        var memberMap = members.ToDictionary(m => m.Id);

        var items = pagedDocs.Select(d =>
        {
            memberMap.TryGetValue(d.MemberId, out var member);
            return MapToResponse(d, member ?? new Member { LegalFirstName = "Unknown", LegalLastName = "" }, 1, 1);
        }).ToList();

        return new PagedResponse<DocumentResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<DocumentResponse> UploadAsync(UploadDocumentRequest request)
    {
        var member = await _unitOfWork.Members.GetByIdAsync(request.MemberId);
        if (member == null)
        {
            throw new NotFoundException("Member", request.MemberId);
        }

        var document = new Document
        {
            Id = Guid.NewGuid(),
            MemberId = request.MemberId,
            OrganizationId = _tenantContext.OrganizationId,
            Category = request.Category,
            DocumentType = request.DocumentType,
            Title = request.Title,
            Status = DocumentStatus.Uploaded,
            ExpiryDate = request.ExpiryDate,
            IsHardCopyNeeded = request.IsHardCopyNeeded,
            CreatedBy = _tenantContext.UserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var storagePath = $"{_tenantContext.OrganizationId}/{request.MemberId}/{request.Category}";
        var filePath = await _fileStorage.UploadAsync(request.FileStream, storagePath, request.FileName);

        var version = new DocumentVersion
        {
            Id = Guid.NewGuid(),
            DocumentId = document.Id,
            VersionNumber = 1,
            FileName = request.FileName,
            FilePath = filePath,
            FileSize = request.FileSize,
            MimeType = request.MimeType,
            UploadedBy = _tenantContext.UserId,
            UploadedAt = DateTime.UtcNow
        };

        document.CurrentVersionId = version.Id;

        await _unitOfWork.Documents.AddAsync(document);
        await _unitOfWork.DocumentVersions.AddAsync(version);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogAsync(
            _tenantContext.OrganizationId,
            _tenantContext.UserId,
            "Document.Uploaded",
            "Document",
            document.Id,
            $"Uploaded document: {document.Title} (v1)");

        _logger.LogInformation("Document {DocumentId} uploaded for member {MemberId}", document.Id, request.MemberId);

        return MapToResponse(document, member, 1, 1);
    }

    public async Task<DocumentResponse> GetByIdAsync(Guid id)
    {
        var document = await _unitOfWork.Documents.FirstOrDefaultAsync(
            d => d.Id == id && d.OrganizationId == _tenantContext.OrganizationId);

        if (document == null)
        {
            throw new NotFoundException("Document", id);
        }

        var member = await _unitOfWork.Members.GetByIdAsync(document.MemberId);
        var versionCount = await _unitOfWork.DocumentVersions.CountAsync(v => v.DocumentId == id);

        return MapToResponse(document, member!, versionCount, versionCount);
    }

    public async Task<DocumentResponse> VerifyAsync(Guid id, string? notes = null)
    {
        var document = await _unitOfWork.Documents.FirstOrDefaultAsync(
            d => d.Id == id && d.OrganizationId == _tenantContext.OrganizationId);

        if (document == null)
        {
            throw new NotFoundException("Document", id);
        }

        if (document.Status != DocumentStatus.Uploaded && document.Status != DocumentStatus.UnderReview)
        {
            throw new BusinessRuleException(
                $"Cannot verify a document with status '{document.Status}'. Only 'Uploaded' or 'UnderReview' documents can be verified.");
        }

        document.Status = DocumentStatus.Verified;
        document.VerifiedBy = _tenantContext.UserId;
        document.VerifiedAt = DateTime.UtcNow;
        document.VerificationNotes = notes;
        document.UpdatedAt = DateTime.UtcNow;
        document.UpdatedBy = _tenantContext.UserId;

        _unitOfWork.Documents.Update(document);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogAsync(
            _tenantContext.OrganizationId,
            _tenantContext.UserId,
            "Document.Verified",
            "Document",
            document.Id,
            $"Verified document: {document.Title}");

        var member = await _unitOfWork.Members.GetByIdAsync(document.MemberId);
        var versionCount = await _unitOfWork.DocumentVersions.CountAsync(v => v.DocumentId == id);

        return MapToResponse(document, member!, versionCount, versionCount);
    }

    public async Task<DocumentResponse> RejectAsync(Guid id, string? reason = null)
    {
        var document = await _unitOfWork.Documents.FirstOrDefaultAsync(
            d => d.Id == id && d.OrganizationId == _tenantContext.OrganizationId);

        if (document == null)
        {
            throw new NotFoundException("Document", id);
        }

        if (document.Status != DocumentStatus.Uploaded && document.Status != DocumentStatus.UnderReview)
        {
            throw new BusinessRuleException(
                $"Cannot reject a document with status '{document.Status}'. Only 'Uploaded' or 'UnderReview' documents can be rejected.");
        }

        document.Status = DocumentStatus.Rejected;
        document.VerificationNotes = reason;
        document.UpdatedAt = DateTime.UtcNow;
        document.UpdatedBy = _tenantContext.UserId;

        _unitOfWork.Documents.Update(document);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogAsync(
            _tenantContext.OrganizationId,
            _tenantContext.UserId,
            "Document.Rejected",
            "Document",
            document.Id,
            $"Rejected document: {document.Title}. Reason: {reason}");

        var member = await _unitOfWork.Members.GetByIdAsync(document.MemberId);
        var versionCount = await _unitOfWork.DocumentVersions.CountAsync(v => v.DocumentId == id);

        return MapToResponse(document, member!, versionCount, versionCount);
    }

    public async Task<IReadOnlyList<DocumentResponse>> GetExpiringDocumentsAsync(int daysThreshold = 30)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(daysThreshold);
        var documents = await _unitOfWork.Documents.FindAsync(
            d => d.OrganizationId == _tenantContext.OrganizationId &&
                 d.ExpiryDate != null &&
                 d.ExpiryDate <= cutoffDate &&
                 d.Status == DocumentStatus.Verified &&
                 !d.IsDeleted);

        var responses = new List<DocumentResponse>();
        foreach (var doc in documents)
        {
            var member = await _unitOfWork.Members.GetByIdAsync(doc.MemberId);
            var versionCount = await _unitOfWork.DocumentVersions.CountAsync(v => v.DocumentId == doc.Id);
            responses.Add(MapToResponse(doc, member!, versionCount, versionCount));
        }

        return responses;
    }

    public async Task<Stream> DownloadAsync(Guid id)
    {
        var document = await _unitOfWork.Documents.FirstOrDefaultAsync(
            d => d.Id == id && d.OrganizationId == _tenantContext.OrganizationId);

        if (document == null)
        {
            throw new NotFoundException("Document", id);
        }

        var currentVersion = document.CurrentVersionId.HasValue
            ? await _unitOfWork.DocumentVersions.GetByIdAsync(document.CurrentVersionId.Value)
            : null;

        if (currentVersion == null)
        {
            throw new NotFoundException("DocumentVersion", document.CurrentVersionId ?? Guid.Empty);
        }

        await _auditService.LogAsync(
            _tenantContext.OrganizationId,
            _tenantContext.UserId,
            "Document.Downloaded",
            "Document",
            document.Id,
            $"Downloaded document: {document.Title} (v{currentVersion.VersionNumber})");

        return await _fileStorage.DownloadAsync(currentVersion.FilePath);
    }

    public async Task<DocumentResponse> ReuploadAsync(Guid documentId, UploadDocumentRequest request)
    {
        var document = await _unitOfWork.Documents.FirstOrDefaultAsync(
            d => d.Id == documentId && d.OrganizationId == _tenantContext.OrganizationId);

        if (document == null)
        {
            throw new NotFoundException("Document", documentId);
        }

        var latestVersionNumber = (await _unitOfWork.DocumentVersions.FindAsync(v => v.DocumentId == documentId))
            .Max(v => v.VersionNumber);

        var storagePath = $"{_tenantContext.OrganizationId}/{document.MemberId}/{document.Category}";
        var filePath = await _fileStorage.UploadAsync(request.FileStream, storagePath, request.FileName);

        var newVersion = new DocumentVersion
        {
            Id = Guid.NewGuid(),
            DocumentId = documentId,
            VersionNumber = latestVersionNumber + 1,
            FileName = request.FileName,
            FilePath = filePath,
            FileSize = request.FileSize,
            MimeType = request.MimeType,
            UploadedBy = _tenantContext.UserId,
            UploadedAt = DateTime.UtcNow
        };

        document.CurrentVersionId = newVersion.Id;
        document.Status = DocumentStatus.Uploaded;
        document.UpdatedAt = DateTime.UtcNow;
        document.UpdatedBy = _tenantContext.UserId;

        if (request.ExpiryDate.HasValue)
        {
            document.ExpiryDate = request.ExpiryDate;
        }

        await _unitOfWork.DocumentVersions.AddAsync(newVersion);
        _unitOfWork.Documents.Update(document);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogAsync(
            _tenantContext.OrganizationId,
            _tenantContext.UserId,
            "Document.Reuploaded",
            "Document",
            document.Id,
            $"Reuploaded document: {document.Title} (v{newVersion.VersionNumber})");

        var member = await _unitOfWork.Members.GetByIdAsync(document.MemberId);
        var totalVersions = latestVersionNumber + 1;

        return MapToResponse(document, member!, totalVersions, newVersion.VersionNumber);
    }

    public async Task DeleteAsync(Guid id)
    {
        var document = await _unitOfWork.Documents.FirstOrDefaultAsync(
            d => d.Id == id && d.OrganizationId == _tenantContext.OrganizationId);

        if (document == null)
        {
            throw new NotFoundException("Document", id);
        }

        document.IsDeleted = true;
        document.DeletedAt = DateTime.UtcNow;
        document.UpdatedAt = DateTime.UtcNow;
        document.UpdatedBy = _tenantContext.UserId;

        _unitOfWork.Documents.Update(document);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogAsync(
            _tenantContext.OrganizationId,
            _tenantContext.UserId,
            "Document.Deleted",
            "Document",
            document.Id,
            $"Deleted document: {document.Title}");

        _logger.LogInformation("Document {DocumentId} soft-deleted by {UserId}", document.Id, _tenantContext.UserId);
    }

    public async Task<IReadOnlyList<DocumentResponse>> GetByMemberAsync(Guid memberId)
    {
        var member = await _unitOfWork.Members.GetByIdAsync(memberId);
        if (member == null)
        {
            throw new NotFoundException("Member", memberId);
        }

        var documents = await _unitOfWork.Documents.FindAsync(
            d => d.MemberId == memberId &&
                 d.OrganizationId == _tenantContext.OrganizationId &&
                 !d.IsDeleted);

        var responses = new List<DocumentResponse>();
        foreach (var doc in documents)
        {
            var versionCount = await _unitOfWork.DocumentVersions.CountAsync(v => v.DocumentId == doc.Id);
            responses.Add(MapToResponse(doc, member, versionCount, versionCount));
        }

        return responses;
    }

    private static DocumentResponse MapToResponse(Document document, Member member, int versionCount, int currentVersion)
    {
        return new DocumentResponse
        {
            Id = document.Id,
            MemberId = document.MemberId,
            MemberName = $"{member.LegalFirstName} {member.LegalLastName}",
            OrganizationId = document.OrganizationId,
            Category = document.Category,
            DocumentType = document.DocumentType,
            Title = document.Title,
            Status = document.Status,
            ExpiryDate = document.ExpiryDate,
            IsHardCopyNeeded = document.IsHardCopyNeeded,
            VerificationNotes = document.VerificationNotes,
            VerifiedAt = document.VerifiedAt,
            VersionCount = versionCount,
            CurrentVersionNumber = currentVersion,
            CreatedAt = document.CreatedAt,
            UpdatedAt = document.UpdatedAt
        };
    }
}
