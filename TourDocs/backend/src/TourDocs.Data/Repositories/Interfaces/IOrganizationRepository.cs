using TourDocs.Domain.Entities;

namespace TourDocs.Data.Repositories.Interfaces;

/// <summary>
/// Repository interface for Organization-specific data operations.
/// </summary>
public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<Organization?> GetWithMembersAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default);
}
