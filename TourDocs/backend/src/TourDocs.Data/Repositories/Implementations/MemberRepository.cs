using Microsoft.EntityFrameworkCore;
using TourDocs.Data.Context;
using TourDocs.Data.Repositories.Interfaces;
using TourDocs.Domain.Entities;

namespace TourDocs.Data.Repositories.Implementations;

public class MemberRepository : Repository<Member>, IMemberRepository
{
    public MemberRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Member>> GetByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(m => m.OrganizationId == organizationId)
            .OrderBy(m => m.LegalLastName)
            .ThenBy(m => m.LegalFirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Member>> SearchAsync(Guid organizationId, string searchTerm, CancellationToken cancellationToken = default)
    {
        var normalizedTerm = searchTerm.ToLower();
        return await DbSet
            .Where(m => m.OrganizationId == organizationId &&
                (m.LegalFirstName.ToLower().Contains(normalizedTerm) ||
                 m.LegalLastName.ToLower().Contains(normalizedTerm) ||
                 (m.Email != null && m.Email.ToLower().Contains(normalizedTerm)) ||
                 (m.PassportNumber != null && m.PassportNumber.ToLower().Contains(normalizedTerm))))
            .OrderBy(m => m.LegalLastName)
            .ToListAsync(cancellationToken);
    }

    public async Task<Member?> GetWithDocumentsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(m => m.Documents)
                .ThenInclude(d => d.CurrentVersion)
            .Include(m => m.TravelHistory)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<Member?> GetByEmailAsync(Guid organizationId, string email, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(m => m.OrganizationId == organizationId && m.Email == email, cancellationToken);
    }
}
