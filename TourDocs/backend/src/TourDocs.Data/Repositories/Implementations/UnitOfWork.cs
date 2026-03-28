using TourDocs.Data.Context;
using TourDocs.Domain.Entities;
using TourDocs.Domain.Interfaces;

namespace TourDocs.Data.Repositories.Implementations;

/// <summary>
/// Unit of Work implementation coordinating repository access and transaction management.
/// Implements the Domain layer's IUnitOfWork interface.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    private IRepository<Organization>? _organizations;
    private IRepository<Member>? _members;
    private IRepository<Document>? _documents;
    private IRepository<DocumentVersion>? _documentVersions;
    private IRepository<DocumentRequest>? _documentRequests;
    private IRepository<Case>? _cases;
    private IRepository<CaseMember>? _caseMembers;
    private IRepository<CaseAccess>? _caseAccesses;
    private IRepository<Checklist>? _checklists;
    private IRepository<ChecklistItem>? _checklistItems;
    private IRepository<HardCopyRequest>? _hardCopyRequests;
    private IRepository<HardCopyHandover>? _hardCopyHandovers;
    private IRepository<TravelHistory>? _travelHistories;
    private IRepository<Notification>? _notifications;
    private IRepository<AuditLog>? _auditLogs;
    private IRepository<Subscription>? _subscriptions;
    private IRepository<OrganizationMember>? _organizationMembers;
    private IRepository<RefreshToken>? _refreshTokens;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IRepository<Organization> Organizations =>
        _organizations ??= new Repository<Organization>(_context);

    public IRepository<Member> Members =>
        _members ??= new Repository<Member>(_context);

    public IRepository<Document> Documents =>
        _documents ??= new Repository<Document>(_context);

    public IRepository<DocumentVersion> DocumentVersions =>
        _documentVersions ??= new Repository<DocumentVersion>(_context);

    public IRepository<DocumentRequest> DocumentRequests =>
        _documentRequests ??= new Repository<DocumentRequest>(_context);

    public IRepository<Case> Cases =>
        _cases ??= new Repository<Case>(_context);

    public IRepository<CaseMember> CaseMembers =>
        _caseMembers ??= new Repository<CaseMember>(_context);

    public IRepository<CaseAccess> CaseAccesses =>
        _caseAccesses ??= new Repository<CaseAccess>(_context);

    public IRepository<Checklist> Checklists =>
        _checklists ??= new Repository<Checklist>(_context);

    public IRepository<ChecklistItem> ChecklistItems =>
        _checklistItems ??= new Repository<ChecklistItem>(_context);

    public IRepository<HardCopyRequest> HardCopyRequests =>
        _hardCopyRequests ??= new Repository<HardCopyRequest>(_context);

    public IRepository<HardCopyHandover> HardCopyHandovers =>
        _hardCopyHandovers ??= new Repository<HardCopyHandover>(_context);

    public IRepository<TravelHistory> TravelHistories =>
        _travelHistories ??= new Repository<TravelHistory>(_context);

    public IRepository<Notification> Notifications =>
        _notifications ??= new Repository<Notification>(_context);

    public IRepository<AuditLog> AuditLogs =>
        _auditLogs ??= new Repository<AuditLog>(_context);

    public IRepository<Subscription> Subscriptions =>
        _subscriptions ??= new Repository<Subscription>(_context);

    public IRepository<OrganizationMember> OrganizationMembers =>
        _organizationMembers ??= new Repository<OrganizationMember>(_context);

    public IRepository<RefreshToken> RefreshTokens =>
        _refreshTokens ??= new Repository<RefreshToken>(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
