using AutoMapper;
using Microsoft.Extensions.Logging;
using TourDocs.Core.DTOs.Subscriptions;
using TourDocs.Core.Exceptions;
using TourDocs.Core.Interfaces;
using TourDocs.Domain.Entities;
using TourDocs.Domain.Interfaces;
using TourDocs.Domain.Enums;

namespace TourDocs.Core.Services;

/// <summary>
/// Business logic service for managing organization subscriptions.
/// </summary>
public class SubscriptionService : ISubscriptionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<SubscriptionService> _logger;

    public SubscriptionService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<SubscriptionService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<SubscriptionResponse> GetByOrganizationAsync(Guid organizationId)
    {
        var subscription = await _unitOfWork.Subscriptions.FirstOrDefaultAsync(
            s => s.OrganizationId == organizationId)
            ?? throw new NotFoundException("Subscription", organizationId);

        return _mapper.Map<SubscriptionResponse>(subscription);
    }

    public async Task<SubscriptionResponse> UpdatePlanAsync(Guid organizationId, UpdateSubscriptionRequest request)
    {
        var subscription = await _unitOfWork.Subscriptions.FirstOrDefaultAsync(
            s => s.OrganizationId == organizationId);

        if (subscription == null)
        {
            subscription = new Subscription
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                Plan = request.Plan,
                Status = "Active",
                CurrentPeriodStart = DateTime.UtcNow,
                CurrentPeriodEnd = DateTime.UtcNow.AddMonths(1),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            SetPlanLimits(subscription);
            await _unitOfWork.Subscriptions.AddAsync(subscription);
        }
        else
        {
            subscription.Plan = request.Plan;
            subscription.UpdatedAt = DateTime.UtcNow;
            SetPlanLimits(subscription);
            _unitOfWork.Subscriptions.Update(subscription);
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Subscription for org {OrgId} updated to plan {Plan}",
            organizationId, request.Plan);

        return _mapper.Map<SubscriptionResponse>(subscription);
    }

    public async Task<SubscriptionUsageResponse> GetUsageAsync(Guid organizationId)
    {
        var subscription = await _unitOfWork.Subscriptions.FirstOrDefaultAsync(
            s => s.OrganizationId == organizationId);

        var memberCount = await _unitOfWork.Members.CountAsync(m => m.OrganizationId == organizationId);
        var caseCount = await _unitOfWork.Cases.CountAsync(c => c.OrganizationId == organizationId);

        return new SubscriptionUsageResponse
        {
            CurrentMembers = memberCount,
            MaxMembers = subscription?.MaxMembers ?? 10,
            CurrentCasesThisMonth = caseCount,
            MaxCasesMonthly = subscription?.MaxCasesMonthly ?? 5,
            CurrentStorageBytes = 0,
            MaxStorageBytes = subscription?.MaxStorageBytes ?? 1073741824
        };
    }

    private static void SetPlanLimits(Subscription subscription)
    {
        switch (subscription.Plan)
        {
            case SubscriptionPlan.Starter:
                subscription.MaxMembers = 25;
                subscription.MaxCasesMonthly = 10;
                subscription.MaxExternalUsers = 2;
                subscription.MaxStorageBytes = 5L * 1024 * 1024 * 1024; // 5 GB
                break;
            case SubscriptionPlan.Professional:
                subscription.MaxMembers = 100;
                subscription.MaxCasesMonthly = 50;
                subscription.MaxExternalUsers = 10;
                subscription.MaxStorageBytes = 25L * 1024 * 1024 * 1024; // 25 GB
                break;
            case SubscriptionPlan.Enterprise:
                subscription.MaxMembers = 10000;
                subscription.MaxCasesMonthly = 1000;
                subscription.MaxExternalUsers = 100;
                subscription.MaxStorageBytes = 500L * 1024 * 1024 * 1024; // 500 GB
                break;
        }
    }
}
