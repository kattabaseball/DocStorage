using TourDocs.Domain.Entities;
using TourDocs.Domain.Enums;

namespace TourDocs.Data.Repositories.Interfaces;

/// <summary>
/// Repository interface for DocumentRequest-specific data operations.
/// </summary>
public interface IDocumentRequestRepository : IRepository<DocumentRequest>
{
    Task<IReadOnlyList<DocumentRequest>> GetByMemberAsync(Guid memberId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DocumentRequest>> GetByCaseAsync(Guid caseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DocumentRequest>> GetByStatusAsync(DocumentRequestStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DocumentRequest>> GetPendingByMemberAsync(Guid memberId, CancellationToken cancellationToken = default);
}
