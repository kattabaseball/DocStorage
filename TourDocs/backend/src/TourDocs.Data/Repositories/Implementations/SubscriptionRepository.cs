using Microsoft.EntityFrameworkCore;
using TourDocs.Data.Context;
using TourDocs.Data.Repositories.Interfaces;
using TourDocs.Domain.Entities;

namespace TourDocs.Data.Repositories.Implementations;

public class SubscriptionRepository : Repository<Subscription>, ISubscriptionRepository
{
    public SubscriptionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Subscription?> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(s => s.Organization)
            .FirstOrDefaultAsync(s => s.OrganizationId == organizationId, cancellationToken);
    }

    public async Task<IReadOnlyList<Subscription>> GetExpiringSubscriptionsAsync(DateTime beforeDate, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(s => s.Organization)
            .Where(s => s.CurrentPeriodEnd <= beforeDate && s.Status == "Active")
            .OrderBy(s => s.CurrentPeriodEnd)
            .ToListAsync(cancellationToken);
    }
}
