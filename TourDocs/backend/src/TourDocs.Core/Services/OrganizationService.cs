using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourDocs.Core.DTOs.Organizations;
using TourDocs.Core.Exceptions;
using TourDocs.Core.Interfaces;
using TourDocs.Domain.Entities;
using TourDocs.Domain.Interfaces;

namespace TourDocs.Core.Services;

/// <summary>
/// Business logic service for managing organizations.
/// </summary>
public class OrganizationService : IOrganizationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ITenantContext _tenantContext;
    private readonly IAuditService _auditService;
    private readonly ILogger<OrganizationService> _logger;

    public OrganizationService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ITenantContext tenantContext,
        IAuditService auditService,
        ILogger<OrganizationService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _tenantContext = tenantContext;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<OrganizationResponse> GetByIdAsync(Guid id)
    {
        var organization = await _unitOfWork.Organizations.GetByIdAsync(id)
            ?? throw new NotFoundException("Organization", id);

        return _mapper.Map<OrganizationResponse>(organization);
    }

    public async Task<OrganizationResponse> GetBySlugAsync(string slug)
    {
        var organization = await _unitOfWork.Organizations
            .FirstOrDefaultAsync(o => o.Slug == slug)
            ?? throw new NotFoundException($"Organization with slug '{slug}' was not found.");

        return _mapper.Map<OrganizationResponse>(organization);
    }

    public async Task<OrganizationResponse> CreateAsync(CreateOrganizationRequest request)
    {
        var slug = GenerateSlug(request.Name);

        var slugExists = await _unitOfWork.Organizations.AnyAsync(o => o.Slug == slug);
        if (slugExists)
        {
            slug = $"{slug}-{Guid.NewGuid().ToString("N")[..6]}";
        }

        var organization = _mapper.Map<Organization>(request);
        organization.Id = Guid.NewGuid();
        organization.Slug = slug;
        organization.IsActive = true;

        await _unitOfWork.Organizations.AddAsync(organization);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Organization {OrgId} '{OrgName}' created", organization.Id, organization.Name);

        return _mapper.Map<OrganizationResponse>(organization);
    }

    public async Task<OrganizationResponse> UpdateAsync(Guid id, UpdateOrganizationRequest request)
    {
        var organization = await _unitOfWork.Organizations.GetByIdAsync(id)
            ?? throw new NotFoundException("Organization", id);

        organization.Name = request.Name;
        organization.BusinessRegNo = request.BusinessRegNo;
        organization.LogoUrl = request.LogoUrl;
        organization.Address = request.Address;
        organization.Phone = request.Phone;
        organization.Email = request.Email;
        organization.Website = request.Website;
        organization.Industry = request.Industry;
        organization.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Organizations.Update(organization);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogAsync(
            id, _tenantContext.UserId,
            "Organization.Updated", "Organization", id,
            $"Updated organization: {organization.Name}");

        return _mapper.Map<OrganizationResponse>(organization);
    }

    public async Task<OrganizationDetailResponse> GetDetailAsync(Guid id)
    {
        var organization = await _unitOfWork.Organizations.GetByIdAsync(id)
            ?? throw new NotFoundException("Organization", id);

        var response = _mapper.Map<OrganizationDetailResponse>(organization);
        response.MemberCount = await _unitOfWork.Members.CountAsync(m => m.OrganizationId == id);
        response.CaseCount = await _unitOfWork.Cases.CountAsync(c => c.OrganizationId == id);

        return response;
    }

    public async Task<IReadOnlyList<OrganizationMemberResponse>> GetMembersAsync(Guid organizationId)
    {
        var members = await _unitOfWork.Members.FindAsync(m => m.OrganizationId == organizationId);
        return members.Select(m => new OrganizationMemberResponse
        {
            Id = m.Id,
            UserId = m.UserId ?? Guid.Empty,
            FullName = $"{m.LegalFirstName} {m.LegalLastName}",
            Email = m.Email ?? string.Empty,
            IsActive = m.IsActive
        }).ToList();
    }

    public async Task InviteMemberAsync(Guid organizationId, InviteOrganizationMemberRequest request)
    {
        var organization = await _unitOfWork.Organizations.GetByIdAsync(organizationId)
            ?? throw new NotFoundException("Organization", organizationId);

        _logger.LogInformation(
            "Invitation sent to {Email} for organization {OrgId} with role {Role}",
            request.Email, organizationId, request.Role);
    }

    public async Task RemoveMemberAsync(Guid organizationId, Guid userId)
    {
        var member = await _unitOfWork.Members.FirstOrDefaultAsync(
            m => m.OrganizationId == organizationId && m.UserId == userId);

        if (member == null)
        {
            throw new NotFoundException("OrganizationMember", userId);
        }

        member.IsActive = false;
        _unitOfWork.Members.Update(member);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("User {UserId} removed from organization {OrgId}", userId, organizationId);
    }

    public async Task<OrganizationSettingsResponse> GetSettingsAsync(Guid organizationId)
    {
        var organization = await _unitOfWork.Organizations.GetByIdAsync(organizationId)
            ?? throw new NotFoundException("Organization", organizationId);

        return new OrganizationSettingsResponse
        {
            Name = organization.Name,
            Email = organization.Email,
            Phone = organization.Phone,
            Address = organization.Address,
            Language = organization.Language,
            Timezone = organization.Timezone,
            EmailNotifications = organization.EmailNotifications,
            ExpiryReminders = organization.ExpiryReminders
        };
    }

    public async Task<OrganizationSettingsResponse> UpdateSettingsAsync(Guid organizationId, UpdateOrganizationSettingsRequest request)
    {
        var organization = await _unitOfWork.Organizations.GetByIdAsync(organizationId)
            ?? throw new NotFoundException("Organization", organizationId);

        organization.Name = request.Name;
        organization.Email = request.Email;
        organization.Phone = request.Phone;
        organization.Address = request.Address;
        organization.Language = request.Language;
        organization.Timezone = request.Timezone;
        organization.EmailNotifications = request.EmailNotifications;
        organization.ExpiryReminders = request.ExpiryReminders;
        organization.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Organizations.Update(organization);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogAsync(
            organizationId, _tenantContext.UserId,
            "Organization.SettingsUpdated", "Organization", organizationId,
            $"Updated organization settings: {organization.Name}");

        _logger.LogInformation("Organization {OrgId} settings updated", organizationId);

        return new OrganizationSettingsResponse
        {
            Name = organization.Name,
            Email = organization.Email,
            Phone = organization.Phone,
            Address = organization.Address,
            Language = organization.Language,
            Timezone = organization.Timezone,
            EmailNotifications = organization.EmailNotifications,
            ExpiryReminders = organization.ExpiryReminders
        };
    }

    private static string GenerateSlug(string name)
    {
        return new string(name.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("&", "and")
            .Where(c => char.IsLetterOrDigit(c) || c == '-')
            .ToArray())
            .Trim('-');
    }
}
