using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TourDocs.Core.Interfaces;
using TourDocs.Infrastructure.BackgroundJobs;
using TourDocs.Infrastructure.Email;
using TourDocs.Infrastructure.FileStorage;
using TourDocs.Infrastructure.Identity;

namespace TourDocs.Infrastructure;

/// <summary>
/// Extension methods for registering infrastructure services in the DI container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds infrastructure services including file storage, identity, email, and external integrations.
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // File storage
        services.Configure<FileStorageSettings>(configuration.GetSection("FileStorage"));
        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        // Identity / JWT
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        // Current user and tenant context
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ITenantContext, TenantContext>();

        // Email (SMTP via MailKit - free)
        services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));
        services.AddScoped<IEmailService, SmtpEmailService>();

        // Background jobs
        services.AddScoped<DocumentExpiryJob>();
        services.AddScoped<NotificationCleanupJob>();

        return services;
    }
}
