using TourDocs.Domain.Entities;

namespace TourDocs.Domain.Interfaces;

/// <summary>
/// Unit of Work pattern for managing transactions across multiple repositories.
/// Defined in Domain layer so both Core and Data can reference it without circular dependencies.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IRepository<Organization> Organizations { get; }
    IRepository<Member> Members { get; }
    IRepository<Document> Documents { get; }
    IRepository<DocumentVersion> DocumentVersions { get; }
    IRepository<DocumentRequest> DocumentRequests { get; }
    IRepository<Case> Cases { get; }
    IRepository<CaseMember> CaseMembers { get; }
    IRepository<CaseAccess> CaseAccesses { get; }
    IRepository<Checklist> Checklists { get; }
    IRepository<ChecklistItem> ChecklistItems { get; }
    IRepository<HardCopyRequest> HardCopyRequests { get; }
    IRepository<HardCopyHandover> HardCopyHandovers { get; }
    IRepository<TravelHistory> TravelHistories { get; }
    IRepository<Notification> Notifications { get; }
    IRepository<AuditLog> AuditLogs { get; }
    IRepository<Subscription> Subscriptions { get; }
    IRepository<OrganizationMember> OrganizationMembers { get; }
    IRepository<RefreshToken> RefreshTokens { get; }

    Task<int> SaveChangesAsync();
}
