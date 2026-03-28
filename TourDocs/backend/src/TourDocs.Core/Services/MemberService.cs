using Microsoft.Extensions.Logging;
using TourDocs.Core.DTOs.Common;
using TourDocs.Core.DTOs.Members;
using TourDocs.Core.Exceptions;
using TourDocs.Core.Interfaces;
using TourDocs.Domain.Entities;
using TourDocs.Domain.Interfaces;

namespace TourDocs.Core.Services;

/// <summary>
/// Business logic service for managing members within an organization.
/// </summary>
public class MemberService : IMemberService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;
    private readonly IAuditService _auditService;
    private readonly ILogger<MemberService> _logger;

    public MemberService(
        IUnitOfWork unitOfWork,
        ITenantContext tenantContext,
        IAuditService auditService,
        ILogger<MemberService> logger)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<MemberResponse> CreateAsync(CreateMemberRequest request)
    {
        if (!string.IsNullOrEmpty(request.Email))
        {
            var existingMember = await _unitOfWork.Members.FirstOrDefaultAsync(
                m => m.Email == request.Email && m.OrganizationId == _tenantContext.OrganizationId);

            if (existingMember != null)
            {
                throw new ValidationException("Email", "A member with this email already exists in the organization.");
            }
        }

        var member = new Member
        {
            Id = Guid.NewGuid(),
            OrganizationId = _tenantContext.OrganizationId,
            LegalFirstName = request.LegalFirstName,
            LegalLastName = request.LegalLastName,
            DateOfBirth = request.DateOfBirth,
            Nationality = request.Nationality,
            NicNumber = request.NicNumber,
            PassportNumber = request.PassportNumber,
            Phone = request.Phone,
            Email = request.Email,
            Address = request.Address,
            Title = request.Title,
            Department = request.Department,
            Specialization = request.Specialization,
            ExternalId = request.ExternalId,
            CustomFields = request.CustomFields,
            Notes = request.Notes,
            IsActive = true,
            CreatedBy = _tenantContext.UserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Members.AddAsync(member);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogAsync(
            _tenantContext.OrganizationId,
            _tenantContext.UserId,
            "Member.Created",
            "Member",
            member.Id,
            $"Created member: {member.LegalFirstName} {member.LegalLastName}");

        _logger.LogInformation("Member {MemberId} created in organization {OrgId}",
            member.Id, _tenantContext.OrganizationId);

        return MapToResponse(member);
    }

    public async Task<MemberResponse> GetByIdAsync(Guid id)
    {
        var member = await _unitOfWork.Members.FirstOrDefaultAsync(
            m => m.Id == id && m.OrganizationId == _tenantContext.OrganizationId);

        if (member == null)
        {
            throw new NotFoundException("Member", id);
        }

        return MapToResponse(member);
    }

    public async Task<MemberResponse> UpdateAsync(Guid id, UpdateMemberRequest request)
    {
        var member = await _unitOfWork.Members.FirstOrDefaultAsync(
            m => m.Id == id && m.OrganizationId == _tenantContext.OrganizationId);

        if (member == null)
        {
            throw new NotFoundException("Member", id);
        }

        if (!string.IsNullOrEmpty(request.Email) && request.Email != member.Email)
        {
            var duplicate = await _unitOfWork.Members.FirstOrDefaultAsync(
                m => m.Email == request.Email && m.OrganizationId == _tenantContext.OrganizationId && m.Id != id);

            if (duplicate != null)
            {
                throw new ValidationException("Email", "A member with this email already exists in the organization.");
            }
        }

        member.LegalFirstName = request.LegalFirstName;
        member.LegalLastName = request.LegalLastName;
        member.DateOfBirth = request.DateOfBirth;
        member.Nationality = request.Nationality;
        member.NicNumber = request.NicNumber;
        member.PassportNumber = request.PassportNumber;
        member.Phone = request.Phone;
        member.Email = request.Email;
        member.Address = request.Address;
        member.Title = request.Title;
        member.Department = request.Department;
        member.Specialization = request.Specialization;
        member.ExternalId = request.ExternalId;
        member.CustomFields = request.CustomFields;
        member.Notes = request.Notes;
        member.UpdatedAt = DateTime.UtcNow;
        member.UpdatedBy = _tenantContext.UserId;

        _unitOfWork.Members.Update(member);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogAsync(
            _tenantContext.OrganizationId,
            _tenantContext.UserId,
            "Member.Updated",
            "Member",
            member.Id,
            $"Updated member: {member.LegalFirstName} {member.LegalLastName}");

        return MapToResponse(member);
    }

    public async Task DeleteAsync(Guid id)
    {
        var member = await _unitOfWork.Members.FirstOrDefaultAsync(
            m => m.Id == id && m.OrganizationId == _tenantContext.OrganizationId);

        if (member == null)
        {
            throw new NotFoundException("Member", id);
        }

        member.IsDeleted = true;
        member.DeletedAt = DateTime.UtcNow;
        member.IsActive = false;
        member.UpdatedAt = DateTime.UtcNow;
        member.UpdatedBy = _tenantContext.UserId;

        _unitOfWork.Members.Update(member);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogAsync(
            _tenantContext.OrganizationId,
            _tenantContext.UserId,
            "Member.Deleted",
            "Member",
            member.Id,
            $"Soft-deleted member: {member.LegalFirstName} {member.LegalLastName}");

        _logger.LogInformation("Member {MemberId} soft-deleted in organization {OrgId}",
            id, _tenantContext.OrganizationId);
    }

    public async Task<PagedResponse<MemberResponse>> GetByOrganizationAsync(
        Guid organizationId, PagedRequest pagedRequest)
    {
        var query = _unitOfWork.Members.Query()
            .Where(m => m.OrganizationId == organizationId && !m.IsDeleted);

        if (!string.IsNullOrWhiteSpace(pagedRequest.SearchTerm))
        {
            var term = pagedRequest.SearchTerm.ToLower();
            query = query.Where(m =>
                m.LegalFirstName.ToLower().Contains(term) ||
                m.LegalLastName.ToLower().Contains(term) ||
                (m.Email != null && m.Email.ToLower().Contains(term)));
        }

        var totalCount = query.Count();

        var members = query
            .OrderBy(m => m.LegalLastName)
            .ThenBy(m => m.LegalFirstName)
            .Skip((pagedRequest.PageNumber - 1) * pagedRequest.PageSize)
            .Take(pagedRequest.PageSize)
            .ToList();

        return new PagedResponse<MemberResponse>
        {
            Items = members.Select(MapToResponse).ToList(),
            TotalCount = totalCount,
            PageNumber = pagedRequest.PageNumber,
            PageSize = pagedRequest.PageSize
        };
    }

    public async Task<PagedResponse<MemberResponse>> SearchAsync(string query, PagedRequest pagedRequest)
    {
        var searchTerm = query.ToLower();
        var membersQuery = _unitOfWork.Members.Query()
            .Where(m => m.OrganizationId == _tenantContext.OrganizationId && !m.IsDeleted)
            .Where(m =>
                m.LegalFirstName.ToLower().Contains(searchTerm) ||
                m.LegalLastName.ToLower().Contains(searchTerm) ||
                (m.Email != null && m.Email.ToLower().Contains(searchTerm)) ||
                (m.NicNumber != null && m.NicNumber.Contains(searchTerm)) ||
                (m.PassportNumber != null && m.PassportNumber.Contains(searchTerm)) ||
                (m.Phone != null && m.Phone.Contains(searchTerm)));

        var totalCount = membersQuery.Count();

        var members = membersQuery
            .OrderBy(m => m.LegalLastName)
            .ThenBy(m => m.LegalFirstName)
            .Skip((pagedRequest.PageNumber - 1) * pagedRequest.PageSize)
            .Take(pagedRequest.PageSize)
            .ToList();

        return new PagedResponse<MemberResponse>
        {
            Items = members.Select(MapToResponse).ToList(),
            TotalCount = totalCount,
            PageNumber = pagedRequest.PageNumber,
            PageSize = pagedRequest.PageSize
        };
    }

    private static MemberResponse MapToResponse(Member member)
    {
        return new MemberResponse
        {
            Id = member.Id,
            OrganizationId = member.OrganizationId,
            UserId = member.UserId,
            LegalFirstName = member.LegalFirstName,
            LegalLastName = member.LegalLastName,
            DateOfBirth = member.DateOfBirth,
            Nationality = member.Nationality,
            NicNumber = member.NicNumber,
            PassportNumber = member.PassportNumber,
            Phone = member.Phone,
            Email = member.Email,
            Address = member.Address,
            Title = member.Title,
            Department = member.Department,
            Specialization = member.Specialization,
            ExternalId = member.ExternalId,
            CustomFields = member.CustomFields,
            ProfilePhotoUrl = member.ProfilePhotoUrl,
            Notes = member.Notes,
            IsActive = member.IsActive,
            CreatedAt = member.CreatedAt,
            UpdatedAt = member.UpdatedAt,
            DocumentCount = member.Documents?.Count ?? 0
        };
    }
}
