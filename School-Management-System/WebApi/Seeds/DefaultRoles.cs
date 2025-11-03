using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace WebApi.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            try
            {
                await roleManager.CreateAsync(new ApplicationRole {Description ="This role is for Super Admin" ,Name = "Super Admin" });
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
