using TourDocs.Domain.Entities;

namespace TourDocs.Data.Repositories.Interfaces;

/// <summary>
/// Repository interface for Member-specific data operations.
/// </summary>
public interface IMemberRepository : IRepository<Member>
{
    Task<IReadOnlyList<Member>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Member>> SearchAsync(Guid organizationId, string searchTerm, CancellationToken cancellationToken = default);
    Task<Member?> GetWithDocumentsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Member?> GetByEmailAsync(Guid organizationId, string email, CancellationToken cancellationToken = default);
}
