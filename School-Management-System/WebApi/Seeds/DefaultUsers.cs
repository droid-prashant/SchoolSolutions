using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace WebApi.Seeds
{
    public class DefaultUsers
    {
        public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            try
            {
                var defaultUser = new ApplicationUser
                {
                    UserName = "smsMaster",
                    Email = "superadmin@gmail.com",
                    PhoneNumber = "1234567890",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    FirstName = "sms",
                    LastName = "master",
                    ShortName = "sms master",
                    Gender = "male",
                    DateOfBirth = DateTime.UtcNow.AddYears(-20),
                    IsActive = true
                };

                var existingDefaultUser = await userManager.FindByNameAsync(defaultUser.UserName);
                if (existingDefaultUser == null)
                {
                    var user = await userManager.FindByEmailAsync(defaultUser.Email);
                    if (user == null)
                    {
                        var res = await userManager.CreateAsync(defaultUser, "P@ssw0rd123");
                        if (res.Succeeded)
                        {
                            existingDefaultUser = defaultUser;
                        }
                    }
                }

                existingDefaultUser ??= await userManager.FindByNameAsync(defaultUser.UserName);
                if (existingDefaultUser != null && !await userManager.IsInRoleAsync(existingDefaultUser, "SuperAdmin"))
                {
                    await userManager.AddToRoleAsync(existingDefaultUser, "SuperAdmin");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
