using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace WebApi.Seeds
{
    public class SeedData
    {
        public static async Task InitializeDefaultData(IHost host)
        {
            var scopeFactory = host.Services.GetService<IServiceScopeFactory>();
            if (scopeFactory != null)
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    try
                    {
                        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
                        await DefaultRoles.SeedRolesAsync(userManager, roleManager);
                        await DefaultUsers.SeedAdminAsync(userManager, roleManager);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
