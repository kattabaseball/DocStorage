using System.Linq.Expressions;

namespace TourDocs.Data.Repositories.Interfaces;

/// <summary>
/// Data layer's generic repository interface extending Domain's IRepository
/// with CancellationToken-enabled overloads for EF Core operations.
/// </summary>
/// <typeparam name="T">Entity type.</typeparam>
public interface IRepository<T> : TourDocs.Domain.Interfaces.IRepository<T> where T : class
{
    /// <summary>
    /// Retrieves an entity by its unique identifier with cancellation support.
    /// </summary>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all entities with cancellation support.
    /// </summary>
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Finds entities matching the specified predicate with cancellation support.
    /// </summary>
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the first entity matching the predicate, or null, with cancellation support.
    /// </summary>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);

    /// <summary>
    /// Returns whether any entity matches the predicate, with cancellation support.
    /// </summary>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the count of entities matching an optional predicate with cancellation support.
    /// </summary>
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new entity to the repository with cancellation support.
    /// </summary>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken);

    /// <summary>
    /// Adds multiple entities to the repository with cancellation support.
    /// </summary>
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken);
}
