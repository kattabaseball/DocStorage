using Microsoft.EntityFrameworkCore;
using TourDocs.Data.Context;
using TourDocs.Data.Repositories.Interfaces;
using TourDocs.Domain.Entities;
using TourDocs.Domain.Enums;

namespace TourDocs.Data.Repositories.Implementations;

public class HardCopyRepository : Repository<HardCopyRequest>, IHardCopyRepository
{
    public HardCopyRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<HardCopyRequest>> GetByCaseAsync(Guid caseId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(h => h.Document)
            .Include(h => h.Handovers)
            .Where(h => h.CaseId == caseId)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<HardCopyRequest?> GetWithHandoversAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(h => h.Document)
            .Include(h => h.Handovers.OrderBy(ho => ho.RecordedAt))
                .ThenInclude(ho => ho.FromUser)
            .Include(h => h.Handovers)
                .ThenInclude(ho => ho.ToUser)
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<HardCopyRequest>> GetByStatusAsync(HardCopyStatus status, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(h => h.Document)
            .Where(h => h.Status == status)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
