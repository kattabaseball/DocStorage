using Microsoft.EntityFrameworkCore;
using TourDocs.Data.Context;
using TourDocs.Data.Repositories.Interfaces;
using TourDocs.Domain.Entities;
using TourDocs.Domain.Enums;

namespace TourDocs.Data.Repositories.Implementations;

public class CaseRepository : Repository<Case>, ICaseRepository
{
    public CaseRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Case>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(c => c.OrganizationId == organizationId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Case?> GetWithMembersAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(c => c.CaseMembers)
                .ThenInclude(cm => cm.Member)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Case?> GetWithFullDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(c => c.CaseMembers)
                .ThenInclude(cm => cm.Member)
            .Include(c => c.CaseAccesses)
                .ThenInclude(ca => ca.User)
            .Include(c => c.Checklist)
                .ThenInclude(cl => cl!.Items)
            .Include(c => c.DocumentRequests)
            .Include(c => c.HardCopyRequests)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Case>> GetByStatusAsync(Guid organizationId, CaseStatus status, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(c => c.OrganizationId == organizationId && c.Status == status)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
