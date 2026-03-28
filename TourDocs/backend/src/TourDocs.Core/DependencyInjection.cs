using Microsoft.Extensions.DependencyInjection;
using TourDocs.Core.Interfaces;
using TourDocs.Core.Mappings;
using TourDocs.Core.Services;

namespace TourDocs.Core;

/// <summary>
/// Extension methods for registering core layer services in the DI container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds all core business logic services, AutoMapper profiles, and validators.
    /// </summary>
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

        // Business services
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<IMemberService, MemberService>();
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<ICaseService, CaseService>();
        services.AddScoped<IChecklistService, ChecklistService>();
        services.AddScoped<IHardCopyService, HardCopyService>();
        services.AddScoped<IDocumentRequestService, DocumentRequestService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();

        return services;
    }
}
