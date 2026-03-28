using TourDocs.Core.DTOs.Common;
using TourDocs.Core.DTOs.Members;

namespace TourDocs.Core.Interfaces;

/// <summary>
/// Service contract for member management operations.
/// </summary>
public interface IMemberService
{
    Task<MemberResponse> CreateAsync(CreateMemberRequest request);
    Task<MemberResponse> GetByIdAsync(Guid id);
    Task<MemberResponse> UpdateAsync(Guid id, UpdateMemberRequest request);
    Task DeleteAsync(Guid id);
    Task<PagedResponse<MemberResponse>> GetByOrganizationAsync(Guid organizationId, PagedRequest pagedRequest);
    Task<PagedResponse<MemberResponse>> SearchAsync(string query, PagedRequest pagedRequest);
}
