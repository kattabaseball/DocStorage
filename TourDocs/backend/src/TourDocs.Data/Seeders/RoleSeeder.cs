using Microsoft.AspNetCore.Identity;

namespace TourDocs.Data.Seeders;

/// <summary>
/// Seeds ASP.NET Identity roles required by the application.
/// </summary>
public static class RoleSeeder
{
    private static readonly string[] Roles =
    {
        "OrgOwner",
        "OrgMember",
        "CaseManager",
        "DocumentHandler",
        "Member"
    };

    public static async Task SeedAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        foreach (var roleName in Roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new IdentityRole<Guid>
                {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    NormalizedName = roleName.ToUpperInvariant()
                };

                var result = await roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Failed to create role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}
