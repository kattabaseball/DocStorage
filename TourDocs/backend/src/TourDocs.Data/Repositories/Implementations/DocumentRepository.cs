using Microsoft.EntityFrameworkCore;
using TourDocs.Data.Context;
using TourDocs.Data.Repositories.Interfaces;
using TourDocs.Domain.Entities;
using TourDocs.Domain.Enums;

namespace TourDocs.Data.Repositories.Implementations;

public class DocumentRepository : Repository<Document>, IDocumentRepository
{
    public DocumentRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Document>> GetByMemberAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(d => d.CurrentVersion)
            .Where(d => d.MemberId == memberId)
            .OrderByDescending(d => d.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Document>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(d => d.Member)
            .Include(d => d.CurrentVersion)
            .Where(d => d.OrganizationId == organizationId)
            .OrderByDescending(d => d.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Document?> GetWithVersionsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(d => d.Versions.OrderByDescending(v => v.VersionNumber))
            .Include(d => d.Member)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Document>> GetExpiringDocumentsAsync(DateTime beforeDate, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(d => d.Member)
            .Where(d => d.ExpiryDate != null && d.ExpiryDate <= beforeDate && d.Status != DocumentStatus.Expired)
            .OrderBy(d => d.ExpiryDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Document>> GetByStatusAsync(Guid organizationId, DocumentStatus status, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(d => d.Member)
            .Where(d => d.OrganizationId == organizationId && d.Status == status)
            .OrderByDescending(d => d.UpdatedAt)
            .ToListAsync(cancellationToken);
    }
}
