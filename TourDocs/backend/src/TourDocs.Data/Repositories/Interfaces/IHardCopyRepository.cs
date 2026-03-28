using TourDocs.Domain.Entities;
using TourDocs.Domain.Enums;

namespace TourDocs.Data.Repositories.Interfaces;

/// <summary>
/// Repository interface for HardCopyRequest-specific data operations.
/// </summary>
public interface IHardCopyRepository : IRepository<HardCopyRequest>
{
    Task<IReadOnlyList<HardCopyRequest>> GetByCaseAsync(Guid caseId, CancellationToken cancellationToken = default);
    Task<HardCopyRequest?> GetWithHandoversAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HardCopyRequest>> GetByStatusAsync(HardCopyStatus status, CancellationToken cancellationToken = default);
}
