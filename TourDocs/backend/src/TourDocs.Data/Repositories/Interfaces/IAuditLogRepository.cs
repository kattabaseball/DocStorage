using TourDocs.Domain.Entities;

namespace TourDocs.Data.Repositories.Interfaces;

/// <summary>
/// Repository interface for AuditLog-specific data operations.
/// </summary>
public interface IAuditLogRepository : IRepository<AuditLog>
{
    Task<IReadOnlyList<AuditLog>> GetByOrganizationAsync(Guid organizationId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AuditLog>> GetByEntityAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AuditLog>> GetByUserAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default);
}
