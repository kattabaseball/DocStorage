using TourDocs.Domain.Entities;
using TourDocs.Domain.Enums;

namespace TourDocs.Data.Repositories.Interfaces;

/// <summary>
/// Repository interface for Document-specific data operations.
/// </summary>
public interface IDocumentRepository : IRepository<Document>
{
    Task<IReadOnlyList<Document>> GetByMemberAsync(Guid memberId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Document>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<Document?> GetWithVersionsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Document>> GetExpiringDocumentsAsync(DateTime beforeDate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Document>> GetByStatusAsync(Guid organizationId, DocumentStatus status, CancellationToken cancellationToken = default);
}
