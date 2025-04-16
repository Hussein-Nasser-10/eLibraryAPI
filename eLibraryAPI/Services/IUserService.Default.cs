
using eLibraryAPI.Models.Dtos;
using eLibraryAPI.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

        public async Task<bool> deleteUser(string userGuid)
        {
            var user = await _userManager.FindByIdAsync(userGuid);
            if (user == null)
            {
                // User not found
                return false;
            }
            
            // Delete the user
            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<List<UserDto>> getUsers(string adminId)
        {
            var users = await _userManager.Users
                .Where(x => x.Id != adminId)
            .Select(x => new UserDto
            {
                Id = x.Id,
                UserName = x.UserName,
                Email = x.Email
            }).ToListAsync();

            return users;
        }
    }
}
