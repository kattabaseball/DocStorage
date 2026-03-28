namespace TourDocs.Data.Repositories.Interfaces;

/// <summary>
/// Unit of Work pattern interface exposing all repositories and transaction management.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IOrganizationRepository Organizations { get; }
    IMemberRepository Members { get; }
    IDocumentRepository Documents { get; }
    ICaseRepository Cases { get; }
    IChecklistRepository Checklists { get; }
    IHardCopyRepository HardCopyRequests { get; }
    IDocumentRequestRepository DocumentRequests { get; }
    IAuditLogRepository AuditLogs { get; }
    INotificationRepository Notifications { get; }
    ISubscriptionRepository Subscriptions { get; }

    /// <summary>
    /// Persists all pending changes to the database.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a new database transaction.
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current transaction.
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
