using Microsoft.EntityFrameworkCore;
using TourDocs.Data.Context;
using TourDocs.Data.Repositories.Interfaces;
using TourDocs.Domain.Entities;
using TourDocs.Domain.Enums;

namespace TourDocs.Data.Repositories.Implementations;

public class DocumentRequestRepository : Repository<DocumentRequest>, IDocumentRequestRepository
{
    public DocumentRequestRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<DocumentRequest>> GetByMemberAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(dr => dr.RequestedByUser)
            .Where(dr => dr.MemberId == memberId)
            .OrderByDescending(dr => dr.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DocumentRequest>> GetByCaseAsync(Guid caseId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(dr => dr.Member)
            .Include(dr => dr.RequestedByUser)
            .Where(dr => dr.CaseId == caseId)
            .OrderByDescending(dr => dr.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DocumentRequest>> GetByStatusAsync(DocumentRequestStatus status, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(dr => dr.Member)
            .Where(dr => dr.Status == status)
            .OrderByDescending(dr => dr.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DocumentRequest>> GetPendingByMemberAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(dr => dr.RequestedByUser)
            .Where(dr => dr.MemberId == memberId &&
                   (dr.Status == DocumentRequestStatus.Requested || dr.Status == DocumentRequestStatus.InProgress))
            .OrderBy(dr => dr.Urgency)
            .ThenByDescending(dr => dr.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
