using Infrastructure.Identity;
using Infrastructure.Persistance;
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
                        var dbContext = services.GetRequiredService<ApplicationDbContext>();
                        await DefaultRoles.SeedRolesAsync(userManager, roleManager, dbContext);
                        await DefaultUsers.SeedAdminAsync(userManager, roleManager);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
