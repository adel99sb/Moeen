using Microsoft.AspNetCore.Identity;
using Moeen.Api.Core.Constants;

namespace Moeen.Api.infrastructure.Data
{
    public static class AppSeeder
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
        {
            foreach (var role in Enum.GetNames(typeof(Roles)))
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>
                    {
                        Id = Guid.NewGuid(),
                        Name = role,
                        NormalizedName = role.ToUpper()
                    });
                }
            }
        }
    }
}
