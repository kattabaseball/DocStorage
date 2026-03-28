namespace TourDocs.Domain.Common;

/// <summary>
/// Interface for entities that support soft deletion instead of permanent removal.
/// </summary>
public interface ISoftDelete
{
    /// <summary>
    /// Indicates whether the entity has been soft-deleted.
    /// </summary>
    bool IsDeleted { get; set; }

    /// <summary>
    /// UTC timestamp when the entity was soft-deleted.
    /// </summary>
    DateTime? DeletedAt { get; set; }
}
