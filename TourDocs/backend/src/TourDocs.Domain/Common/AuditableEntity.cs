namespace TourDocs.Domain.Common;

/// <summary>
/// Extends BaseEntity with audit trail properties tracking who created and modified the entity.
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    /// <summary>
    /// Identifier of the user who created the entity.
    /// </summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>
    /// Identifier of the user who last updated the entity.
    /// </summary>
    public Guid? UpdatedBy { get; set; }
}
