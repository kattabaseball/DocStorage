namespace TourDocs.Core.Interfaces;

/// <summary>
/// Service for recording audit trail entries.
/// </summary>
public interface IAuditService
{
    Task LogAsync(Guid? organizationId, Guid? userId, string action, string? entityType = null,
        Guid? entityId = null, string? details = null, string? ipAddress = null, string? userAgent = null);
}
