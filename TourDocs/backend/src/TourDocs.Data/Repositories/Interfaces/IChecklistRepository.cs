using TourDocs.Domain.Entities;

namespace TourDocs.Data.Repositories.Interfaces;

/// <summary>
/// Repository interface for Checklist-specific data operations.
/// </summary>
public interface IChecklistRepository : IRepository<Checklist>
{
    Task<IReadOnlyList<Checklist>> GetByCountryAsync(string countryCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Checklist>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<Checklist?> GetWithItemsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Checklist>> GetSystemChecklistsAsync(CancellationToken cancellationToken = default);
}
