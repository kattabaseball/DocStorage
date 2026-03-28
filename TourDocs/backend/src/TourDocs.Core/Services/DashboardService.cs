using AutoMapper;
using Microsoft.Extensions.Logging;
using TourDocs.Core.DTOs.Dashboard;
using TourDocs.Core.Interfaces;
using TourDocs.Domain.Enums;
using TourDocs.Domain.Interfaces;

namespace TourDocs.Core.Services;

/// <summary>
/// Business logic service for aggregating dashboard data.
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ITenantContext _tenantContext;

    public DashboardService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _tenantContext = tenantContext;
    }

    public async Task<DashboardSummaryResponse> GetSummaryAsync(Guid organizationId)
    {
        var expiryThreshold = DateTime.UtcNow.AddDays(30);

        return new DashboardSummaryResponse
        {
            TotalMembers = await _unitOfWork.Members.CountAsync(
                m => m.OrganizationId == organizationId),
            ActiveCases = await _unitOfWork.Cases.CountAsync(
                c => c.OrganizationId == organizationId &&
                     c.Status != CaseStatus.Completed &&
                     c.Status != CaseStatus.Cancelled),
            TotalDocuments = await _unitOfWork.Documents.CountAsync(
                d => d.OrganizationId == organizationId),
            PendingVerifications = await _unitOfWork.Documents.CountAsync(
                d => d.OrganizationId == organizationId &&
                     d.Status == DocumentStatus.UnderReview),
            ExpiringDocuments = await _unitOfWork.Documents.CountAsync(
                d => d.OrganizationId == organizationId &&
                     d.ExpiryDate != null &&
                     d.ExpiryDate <= expiryThreshold &&
                     d.Status != DocumentStatus.Expired),
            PendingHardCopyRequests = await _unitOfWork.HardCopyRequests.CountAsync(
                h => h.Status == HardCopyStatus.Requested || h.Status == HardCopyStatus.Acknowledged),
            PendingDocumentRequests = await _unitOfWork.DocumentRequests.CountAsync(
                dr => dr.Status == DocumentRequestStatus.Requested ||
                      dr.Status == DocumentRequestStatus.InProgress),
            UnreadNotifications = await _unitOfWork.Notifications.CountAsync(
                n => n.UserId == _tenantContext.UserId && !n.IsRead),
            VerifiedDocuments = await _unitOfWork.Documents.CountAsync(
                d => d.OrganizationId == organizationId &&
                     d.Status == DocumentStatus.Verified),
            UploadedDocuments = await _unitOfWork.Documents.CountAsync(
                d => d.OrganizationId == organizationId &&
                     d.Status == DocumentStatus.Uploaded),
            RejectedDocuments = await _unitOfWork.Documents.CountAsync(
                d => d.OrganizationId == organizationId &&
                     d.Status == DocumentStatus.Rejected),
            ExpiredDocuments = await _unitOfWork.Documents.CountAsync(
                d => d.OrganizationId == organizationId &&
                     d.Status == DocumentStatus.Expired)
        };
    }

    public async Task<IReadOnlyList<ExpiringDocumentResponse>> GetExpiringDocumentsAsync(
        Guid organizationId, int daysAhead = 30)
    {
        var threshold = DateTime.UtcNow.AddDays(daysAhead);
        var documents = await _unitOfWork.Documents.FindAsync(
            d => d.OrganizationId == organizationId &&
                 d.ExpiryDate != null &&
                 d.ExpiryDate <= threshold &&
                 d.Status != DocumentStatus.Expired);

        return documents
            .OrderBy(d => d.ExpiryDate)
            .Select(d => new ExpiringDocumentResponse
            {
                DocumentId = d.Id,
                Title = d.Title,
                MemberName = $"{d.Member?.LegalFirstName} {d.Member?.LegalLastName}".Trim(),
                ExpiryDate = d.ExpiryDate,
                DaysUntilExpiry = d.ExpiryDate.HasValue
                    ? (int)(d.ExpiryDate.Value - DateTime.UtcNow).TotalDays
                    : 0
            })
            .ToList();
    }

    public async Task<IReadOnlyList<CaseReadinessResponse>> GetCaseReadinessAsync(
        Guid organizationId, int count = 5)
    {
        var cases = await _unitOfWork.Cases.FindAsync(
            c => c.OrganizationId == organizationId &&
                 c.Status != CaseStatus.Completed &&
                 c.Status != CaseStatus.Cancelled);

        var results = new List<CaseReadinessResponse>();
        foreach (var caseEntity in cases.OrderByDescending(c => c.CreatedAt).Take(count))
        {
            var checklistItems = caseEntity.ChecklistId.HasValue
                ? await _unitOfWork.ChecklistItems.CountAsync(ci => ci.ChecklistId == caseEntity.ChecklistId.Value)
                : 0;

            var caseMembers = await _unitOfWork.CaseMembers.FindAsync(cm => cm.CaseId == caseEntity.Id);
            var memberIds = caseMembers.Select(cm => cm.MemberId).ToList();

            var verifiedDocs = 0;
            var totalRequired = checklistItems * memberIds.Count;

            if (totalRequired > 0)
            {
                verifiedDocs = await _unitOfWork.Documents.CountAsync(
                    d => memberIds.Contains(d.MemberId) &&
                         d.Status == DocumentStatus.Verified);
            }

            var readyPercent = totalRequired > 0
                ? (int)Math.Round(100.0 * verifiedDocs / totalRequired)
                : 0;

            results.Add(new CaseReadinessResponse
            {
                CaseId = caseEntity.Id,
                CaseName = caseEntity.Name,
                ReadyPercent = Math.Min(readyPercent, 100),
                PendingPercent = 100 - Math.Min(readyPercent, 100)
            });
        }

        return results;
    }

    public async Task<IReadOnlyList<RecentActivityResponse>> GetRecentActivityAsync(
        Guid organizationId, int count = 10)
    {
        var logs = await _unitOfWork.AuditLogs.FindAsync(
            a => a.OrganizationId == organizationId);

        return _mapper.Map<IReadOnlyList<RecentActivityResponse>>(
            logs.OrderByDescending(a => a.CreatedAt).Take(count).ToList());
    }
}
