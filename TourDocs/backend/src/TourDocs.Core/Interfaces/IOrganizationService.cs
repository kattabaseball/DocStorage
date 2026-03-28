using TourDocs.Core.DTOs.Organizations;

namespace TourDocs.Core.Interfaces;

/// <summary>
/// Service contract for organization management operations.
/// </summary>
public interface IOrganizationService
{
    Task<OrganizationResponse> GetByIdAsync(Guid id);
    Task<OrganizationResponse> GetBySlugAsync(string slug);
    Task<OrganizationResponse> CreateAsync(CreateOrganizationRequest request);
    Task<OrganizationResponse> UpdateAsync(Guid id, UpdateOrganizationRequest request);
    Task<OrganizationDetailResponse> GetDetailAsync(Guid id);
    Task<IReadOnlyList<OrganizationMemberResponse>> GetMembersAsync(Guid organizationId);
    Task InviteMemberAsync(Guid organizationId, InviteOrganizationMemberRequest request);
    Task RemoveMemberAsync(Guid organizationId, Guid userId);
    Task<OrganizationSettingsResponse> GetSettingsAsync(Guid organizationId);
    Task<OrganizationSettingsResponse> UpdateSettingsAsync(Guid organizationId, UpdateOrganizationSettingsRequest request);
}
