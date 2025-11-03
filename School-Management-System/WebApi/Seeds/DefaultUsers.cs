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
                //Seed Default User
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
                    DateOfBirth = System.DateTime.UtcNow.AddYears(-20)
                };


                if (await userManager.FindByNameAsync(defaultUser.UserName) == null)
                {
                    var user = await userManager.FindByEmailAsync(defaultUser.Email);
                    if (user == null)
                    {
                        var res = await userManager.CreateAsync(defaultUser, "P@ssw0rd123");
                        if (res.Succeeded)
                        {
                            await userManager.AddToRoleAsync(defaultUser, "Super Admin");
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
