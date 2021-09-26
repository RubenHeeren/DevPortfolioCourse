using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Server.Data;
public static class SeedAdministratorRoleAndUser
{
    internal const string AdministratorRoleName = "Administrator";
    internal const string AdministratorUserName = "admin@johndoe.com";

    internal async static Task Seed(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
    {
        await SeedAdministratorRole(roleManager);
        await SeedAdministratorUser(userManager);
    }

    private async static Task SeedAdministratorRole(RoleManager<IdentityRole> roleManager)
    {
        bool administratorRoleExists = await roleManager.RoleExistsAsync(AdministratorRoleName);

        if (administratorRoleExists == false)
        {
            var role = new IdentityRole
            {
                Name = AdministratorRoleName
            };

            await roleManager.CreateAsync(role);
        }
    }

    private async static Task SeedAdministratorUser(UserManager<IdentityUser> userManager)
    {
        bool administratorUserExists = await userManager.FindByEmailAsync(AdministratorUserName) != null;

        if (administratorUserExists == false)
        {
            var user = new IdentityUser
            {
                UserName = AdministratorUserName,
                Email = AdministratorUserName
            };

            // Make sure your Git repo is private if you do this
            IdentityResult identityResult = await userManager.CreateAsync(user, "Password1!");

            if (identityResult.Succeeded)
            {
                await userManager.AddToRoleAsync(user, AdministratorRoleName);
            }
        }
    }
}
