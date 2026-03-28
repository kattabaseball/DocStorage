using Microsoft.EntityFrameworkCore;
using TourDocs.Data.Context;
using TourDocs.Data.Repositories.Interfaces;
using TourDocs.Domain.Entities;

namespace TourDocs.Data.Repositories.Implementations;

public class OrganizationRepository : Repository<Organization>, IOrganizationRepository
{
    public OrganizationRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Organization?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(o => o.Slug == slug, cancellationToken);
    }

    public async Task<Organization?> GetWithMembersAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(o => o.OrganizationMembers)
                .ThenInclude(om => om.User)
            .Include(o => o.Subscription)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(o => o.Slug == slug, cancellationToken);
    }
}
