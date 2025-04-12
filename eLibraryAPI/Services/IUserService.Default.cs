
using eLibraryAPI.Models.Models;
using Microsoft.AspNetCore.Identity;

namespace eLibraryAPI.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<bool> createUser(UserModel userModel)
        {
            var user = new IdentityUser { UserName = userModel.Username, Email = userModel.Email };
            var result = await _userManager.CreateAsync(user, userModel.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Users");
            }
            return result.Succeeded;
        }
    }
}
