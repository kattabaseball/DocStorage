namespace TourDocs.Data.Seeders;

using Microsoft.AspNetCore.Identity;
using TourDocs.Data.Context;
using TourDocs.Domain.Entities;

/// <summary>
/// Master seeder that orchestrates all seeders in the correct dependency order.
/// </summary>
public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        await RoleSeeder.SeedAsync(roleManager);
        await DocumentCategorySeeder.SeedAsync(context);
        await CountryChecklistSeeder.SeedAsync(context);
        await DemoDataSeeder.SeedAsync(context, userManager);
    }
}
