using Microsoft.EntityFrameworkCore;
using TourDocs.Data.Context;
using TourDocs.Data.Repositories.Interfaces;
using TourDocs.Domain.Entities;

namespace TourDocs.Data.Repositories.Implementations;

public class ChecklistRepository : Repository<Checklist>, IChecklistRepository
{
    public ChecklistRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Checklist>> GetByCountryAsync(string countryCode, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(c => c.Items.OrderBy(i => i.SortOrder))
            .Where(c => c.CountryCode == countryCode && c.IsActive)
            .OrderByDescending(c => c.Version)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Checklist>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(c => c.OrganizationId == organizationId && c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Checklist?> GetWithItemsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(c => c.Items.OrderBy(i => i.SortOrder))
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Checklist>> GetSystemChecklistsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(c => c.IsSystem && c.IsActive)
            .OrderBy(c => c.CountryName)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }
}
