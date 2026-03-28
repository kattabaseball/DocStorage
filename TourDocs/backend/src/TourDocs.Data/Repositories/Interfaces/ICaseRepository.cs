using TourDocs.Domain.Entities;
using TourDocs.Domain.Enums;

namespace TourDocs.Data.Repositories.Interfaces;

/// <summary>
/// Repository interface for Case-specific data operations.
/// </summary>
public interface ICaseRepository : IRepository<Case>
{
    Task<IReadOnlyList<Case>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<Case?> GetWithMembersAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Case?> GetWithFullDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Case>> GetByStatusAsync(Guid organizationId, CaseStatus status, CancellationToken cancellationToken = default);
}
