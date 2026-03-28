using TourDocs.Core.DTOs.Cases;
using TourDocs.Core.DTOs.Common;

namespace TourDocs.Core.Interfaces;

/// <summary>
/// Service contract for case management operations.
/// </summary>
public interface ICaseService
{
    Task<PagedResponse<CaseResponse>> GetByOrganizationAsync(Guid organizationId, PagedRequest request);
    Task<CaseResponse> CreateAsync(CreateCaseRequest request);
    Task<CaseResponse> GetByIdAsync(Guid id);
    Task AssignMembersAsync(Guid caseId, IEnumerable<Guid> memberIds);
    Task<double> GetReadinessPercentageAsync(Guid caseId);
    Task GrantAccessAsync(Guid caseId, GrantAccessRequest request);
    Task RevokeAccessAsync(Guid caseAccessId);
}
