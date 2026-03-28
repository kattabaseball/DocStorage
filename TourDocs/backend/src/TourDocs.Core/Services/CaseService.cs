using Microsoft.Extensions.Logging;
using TourDocs.Core.DTOs.Cases;
using TourDocs.Core.DTOs.Common;
using TourDocs.Core.Exceptions;
using TourDocs.Core.Interfaces;
using TourDocs.Domain.Entities;
using TourDocs.Domain.Interfaces;
using TourDocs.Domain.Enums;

namespace TourDocs.Core.Services;

/// <summary>
/// Business logic service for managing cases, member assignments, and access control.
/// </summary>
public class CaseService : ICaseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;
    private readonly IAuditService _auditService;
    private readonly ILogger<CaseService> _logger;

    public CaseService(
        IUnitOfWork unitOfWork,
        ITenantContext tenantContext,
        IAuditService auditService,
        ILogger<CaseService> logger)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<PagedResponse<CaseResponse>> GetByOrganizationAsync(Guid organizationId, PagedRequest request)
    {
        var cases = await _unitOfWork.Cases.FindAsync(c => c.OrganizationId == organizationId);
        var query = cases.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLowerInvariant();
            query = query.Where(c => c.Name.ToLower().Contains(term) ||
                                     (c.DestinationCountry != null && c.DestinationCountry.ToLower().Contains(term)));
        }

        var totalCount = query.Count();
        var pagedCases = query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var items = new List<CaseResponse>();
        foreach (var c in pagedCases)
        {
            var memberCount = await _unitOfWork.CaseMembers.CountAsync(cm => cm.CaseId == c.Id);
            var readiness = await CalculateReadinessAsync(c);
            items.Add(MapToResponse(c, memberCount, readiness));
        }

        return new PagedResponse<CaseResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<CaseResponse> CreateAsync(CreateCaseRequest request)
    {
        var referenceNumber = $"CASE-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";

        var caseEntity = new Case
        {
            Id = Guid.NewGuid(),
            OrganizationId = _tenantContext.OrganizationId,
            Name = request.Name,
            CaseType = request.CaseType,
            ReferenceNumber = referenceNumber,
            DestinationCountry = request.DestinationCountry,
            DestinationCity = request.DestinationCity,
            Venue = request.Venue,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            ContactName = request.ContactName,
            ContactEmail = request.ContactEmail,
            ContactPhone = request.ContactPhone,
            ChecklistId = request.ChecklistId,
            Status = CaseStatus.Draft,
            Description = request.Description,
            Notes = request.Notes,
            CreatedBy = _tenantContext.UserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Cases.AddAsync(caseEntity);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogAsync(
            _tenantContext.OrganizationId,
            _tenantContext.UserId,
            "Case.Created",
            "Case",
            caseEntity.Id,
            $"Created case: {caseEntity.Name} ({referenceNumber})");

        _logger.LogInformation("Case {CaseId} created in organization {OrgId}", caseEntity.Id, _tenantContext.OrganizationId);

        return MapToResponse(caseEntity, 0, 0);
    }

    public async Task<CaseResponse> GetByIdAsync(Guid id)
    {
        var caseEntity = await _unitOfWork.Cases.FirstOrDefaultAsync(
            c => c.Id == id && c.OrganizationId == _tenantContext.OrganizationId);

        if (caseEntity == null)
        {
            throw new NotFoundException("Case", id);
        }

        var memberCount = await _unitOfWork.CaseMembers.CountAsync(cm => cm.CaseId == id);
        var readiness = await CalculateReadinessAsync(caseEntity);

        return MapToResponse(caseEntity, memberCount, readiness);
    }

    public async Task AssignMembersAsync(Guid caseId, IEnumerable<Guid> memberIds)
    {
        var caseEntity = await _unitOfWork.Cases.FirstOrDefaultAsync(
            c => c.Id == caseId && c.OrganizationId == _tenantContext.OrganizationId);

        if (caseEntity == null)
        {
            throw new NotFoundException("Case", caseId);
        }

        var existingAssignments = await _unitOfWork.CaseMembers.FindAsync(cm => cm.CaseId == caseId);
        var existingMemberIds = existingAssignments.Select(cm => cm.MemberId).ToHashSet();

        var newAssignments = new List<CaseMember>();
        foreach (var memberId in memberIds)
        {
            if (existingMemberIds.Contains(memberId))
            {
                continue;
            }

            var member = await _unitOfWork.Members.GetByIdAsync(memberId);
            if (member == null || member.OrganizationId != _tenantContext.OrganizationId)
            {
                throw new NotFoundException("Member", memberId);
            }

            newAssignments.Add(new CaseMember
            {
                Id = Guid.NewGuid(),
                CaseId = caseId,
                MemberId = memberId,
                Status = "Assigned",
                AddedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        if (newAssignments.Count > 0)
        {
            await _unitOfWork.CaseMembers.AddRangeAsync(newAssignments);
            await _unitOfWork.SaveChangesAsync();

            await _auditService.LogAsync(
                _tenantContext.OrganizationId,
                _tenantContext.UserId,
                "Case.MembersAssigned",
                "Case",
                caseId,
                $"Assigned {newAssignments.Count} members to case: {caseEntity.Name}");
        }
    }

    public async Task<CaseResponse> UpdateAsync(Guid id, UpdateCaseRequest request)
    {
        var caseEntity = await _unitOfWork.Cases.FirstOrDefaultAsync(
            c => c.Id == id && c.OrganizationId == _tenantContext.OrganizationId);

        if (caseEntity == null)
        {
            throw new NotFoundException("Case", id);
        }

        caseEntity.Name = request.Name;
        caseEntity.Description = request.Description;
        caseEntity.CaseType = request.CaseType;
        caseEntity.DestinationCountry = request.DestinationCountry;
        caseEntity.DestinationCity = request.DestinationCity;
        caseEntity.StartDate = request.StartDate;
        caseEntity.EndDate = request.EndDate;
        caseEntity.ChecklistId = request.ChecklistId;
        caseEntity.ContactName = request.ContactName;
        caseEntity.ContactEmail = request.ContactEmail;
        caseEntity.ContactPhone = request.ContactPhone;
        caseEntity.Notes = request.Notes;
        caseEntity.UpdatedAt = DateTime.UtcNow;
        caseEntity.UpdatedBy = _tenantContext.UserId;

        _unitOfWork.Cases.Update(caseEntity);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogAsync(
            _tenantContext.OrganizationId,
            _tenantContext.UserId,
            "Case.Updated",
            "Case",
            caseEntity.Id,
            $"Updated case: {caseEntity.Name} ({caseEntity.ReferenceNumber})");

        _logger.LogInformation("Case {CaseId} updated in organization {OrgId}", caseEntity.Id, _tenantContext.OrganizationId);

        var memberCount = await _unitOfWork.CaseMembers.CountAsync(cm => cm.CaseId == id);
        var readiness = await CalculateReadinessAsync(caseEntity);

        return MapToResponse(caseEntity, memberCount, readiness);
    }

    public async Task DeleteAsync(Guid id)
    {
        var caseEntity = await _unitOfWork.Cases.FirstOrDefaultAsync(
            c => c.Id == id && c.OrganizationId == _tenantContext.OrganizationId);

        if (caseEntity == null)
        {
            throw new NotFoundException("Case", id);
        }

        caseEntity.IsDeleted = true;
        caseEntity.DeletedAt = DateTime.UtcNow;
        caseEntity.UpdatedAt = DateTime.UtcNow;
        caseEntity.UpdatedBy = _tenantContext.UserId;

        _unitOfWork.Cases.Update(caseEntity);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogAsync(
            _tenantContext.OrganizationId,
            _tenantContext.UserId,
            "Case.Deleted",
            "Case",
            caseEntity.Id,
            $"Deleted case: {caseEntity.Name} ({caseEntity.ReferenceNumber})");

        _logger.LogInformation("Case {CaseId} soft-deleted in organization {OrgId}", caseEntity.Id, _tenantContext.OrganizationId);
    }

    public async Task RemoveMemberAsync(Guid caseId, Guid memberId)
    {
        var caseEntity = await _unitOfWork.Cases.FirstOrDefaultAsync(
            c => c.Id == caseId && c.OrganizationId == _tenantContext.OrganizationId);

        if (caseEntity == null)
        {
            throw new NotFoundException("Case", caseId);
        }

        var caseMember = await _unitOfWork.CaseMembers.FirstOrDefaultAsync(
            cm => cm.CaseId == caseId && cm.MemberId == memberId);

        if (caseMember == null)
        {
            throw new NotFoundException("CaseMember", memberId);
        }

        _unitOfWork.CaseMembers.Remove(caseMember);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogAsync(
            _tenantContext.OrganizationId,
            _tenantContext.UserId,
            "Case.MemberRemoved",
            "Case",
            caseId,
            $"Removed member {memberId} from case: {caseEntity.Name}");

        _logger.LogInformation("Member {MemberId} removed from case {CaseId}", memberId, caseId);
    }

    public async Task<double> GetReadinessPercentageAsync(Guid caseId)
    {
        var caseEntity = await _unitOfWork.Cases.FirstOrDefaultAsync(
            c => c.Id == caseId && c.OrganizationId == _tenantContext.OrganizationId);

        if (caseEntity == null)
        {
            throw new NotFoundException("Case", caseId);
        }

        return await CalculateReadinessAsync(caseEntity);
    }

    public async Task GrantAccessAsync(Guid caseId, GrantAccessRequest request)
    {
        var caseEntity = await _unitOfWork.Cases.FirstOrDefaultAsync(
            c => c.Id == caseId && c.OrganizationId == _tenantContext.OrganizationId);

        if (caseEntity == null)
        {
            throw new NotFoundException("Case", caseId);
        }

        var existingAccess = await _unitOfWork.CaseAccesses.FirstOrDefaultAsync(
            ca => ca.CaseId == caseId && ca.UserId == request.UserId && ca.IsActive);

        if (existingAccess != null)
        {
            throw new BusinessRuleException("This user already has active access to the case.");
        }

        var access = new CaseAccess
        {
            Id = Guid.NewGuid(),
            CaseId = caseId,
            UserId = request.UserId,
            Role = request.Role,
            Permission = request.Permission,
            GrantedBy = _tenantContext.UserId,
            GrantedAt = DateTime.UtcNow,
            ExpiresAt = request.ExpiresAt,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.CaseAccesses.AddAsync(access);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogAsync(
            _tenantContext.OrganizationId,
            _tenantContext.UserId,
            "Case.AccessGranted",
            "CaseAccess",
            access.Id,
            $"Granted {request.Permission} access to user {request.UserId} for case: {caseEntity.Name}");
    }

    public async Task RevokeAccessAsync(Guid caseAccessId)
    {
        var access = await _unitOfWork.CaseAccesses.GetByIdAsync(caseAccessId);

        if (access == null)
        {
            throw new NotFoundException("CaseAccess", caseAccessId);
        }

        var caseEntity = await _unitOfWork.Cases.FirstOrDefaultAsync(
            c => c.Id == access.CaseId && c.OrganizationId == _tenantContext.OrganizationId);

        if (caseEntity == null)
        {
            throw new ForbiddenException("You do not have permission to revoke access for this case.");
        }

        access.IsActive = false;
        access.RevokedAt = DateTime.UtcNow;
        access.RevokedBy = _tenantContext.UserId;
        access.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.CaseAccesses.Update(access);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogAsync(
            _tenantContext.OrganizationId,
            _tenantContext.UserId,
            "Case.AccessRevoked",
            "CaseAccess",
            caseAccessId,
            $"Revoked access for user {access.UserId} from case: {caseEntity.Name}");
    }

    private async Task<double> CalculateReadinessAsync(Case caseEntity)
    {
        if (!caseEntity.ChecklistId.HasValue)
        {
            return 0;
        }

        var checklistItems = await _unitOfWork.ChecklistItems.FindAsync(
            ci => ci.ChecklistId == caseEntity.ChecklistId.Value && ci.IsRequired);

        if (checklistItems.Count == 0)
        {
            return 100;
        }

        var caseMembers = await _unitOfWork.CaseMembers.FindAsync(cm => cm.CaseId == caseEntity.Id);
        if (caseMembers.Count == 0)
        {
            return 0;
        }

        var totalRequired = checklistItems.Count * caseMembers.Count;
        var fulfilled = 0;

        foreach (var cm in caseMembers)
        {
            var memberDocs = await _unitOfWork.Documents.FindAsync(
                d => d.MemberId == cm.MemberId &&
                     d.OrganizationId == _tenantContext.OrganizationId &&
                     d.Status == DocumentStatus.Verified &&
                     !d.IsDeleted);

            foreach (var item in checklistItems)
            {
                if (memberDocs.Any(d => d.DocumentType == item.DocumentType && d.Category == item.DocumentCategory))
                {
                    fulfilled++;
                }
            }
        }

        return totalRequired > 0 ? Math.Round((double)fulfilled / totalRequired * 100, 2) : 0;
    }

    private static CaseResponse MapToResponse(Case caseEntity, int memberCount, double readiness)
    {
        return new CaseResponse
        {
            Id = caseEntity.Id,
            OrganizationId = caseEntity.OrganizationId,
            Name = caseEntity.Name,
            CaseType = caseEntity.CaseType,
            ReferenceNumber = caseEntity.ReferenceNumber,
            DestinationCountry = caseEntity.DestinationCountry,
            DestinationCity = caseEntity.DestinationCity,
            Venue = caseEntity.Venue,
            StartDate = caseEntity.StartDate,
            EndDate = caseEntity.EndDate,
            ContactName = caseEntity.ContactName,
            ContactEmail = caseEntity.ContactEmail,
            ContactPhone = caseEntity.ContactPhone,
            ChecklistId = caseEntity.ChecklistId,
            Status = caseEntity.Status,
            Description = caseEntity.Description,
            Notes = caseEntity.Notes,
            MemberCount = memberCount,
            ReadinessPercentage = readiness,
            CreatedAt = caseEntity.CreatedAt,
            UpdatedAt = caseEntity.UpdatedAt
        };
    }
}
