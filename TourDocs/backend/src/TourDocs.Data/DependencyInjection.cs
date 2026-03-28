using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TourDocs.Data.Context;
using TourDocs.Data.Repositories.Implementations;
using TourDocs.Domain.Interfaces;

namespace TourDocs.Data;

/// <summary>
/// Extension methods for registering data layer services in the DI container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds data layer services including DbContext, repositories, and Unit of Work.
    /// </summary>
    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    sqlOptions.EnableRetryOnFailure(3);
                }));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        return services;
    }
}
