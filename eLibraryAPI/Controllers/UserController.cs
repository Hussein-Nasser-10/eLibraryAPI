using eLibraryAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eLibraryAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using eLibraryAPI.Services;
namespace eLibraryAPI.Controllers
{
        [Route("api/[controller]")]
        [ApiController]
        [Authorize(Roles = "Admins")]
        public class UserController : ControllerBase
        {
            private readonly ApplicationDbContext _context;
            private readonly UserManager<IdentityUser> _userManager;
            private readonly IUserService _userService;

        public UserController(ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IUserService userService)
            {
                _context = context;
                _userManager = userManager;
                _userService = userService;
            }

            // Search users by name
            [HttpGet("searchByName")]
            public async Task<IActionResult> SearchUsersByName(string name)
            {
                if (string.IsNullOrEmpty(name))
                {
                    return BadRequest("Name cannot be empty.");
                }

                var users = await _context.Users
                    .Where(u => u.Username.Contains(name, StringComparison.OrdinalIgnoreCase))
                    .ToListAsync();
            if (users.Count == 0)
                {
                    return NotFound("No users found with that name.");
                }

                return Ok(users);
            }

        [HttpGet("getUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var userId = _userManager.GetUserId(User);
            var users = await _userService.getUsers(userId);
            return Ok(users);
        }

        [HttpDelete("deleteUser")]
        public async Task<IActionResult> deleteUser(string userId)
        {
            var deleteState = await _userService.deleteUser(userId);
            if (!deleteState)
            {
                return NotFound("user not found");
            }
            return Ok("user deleted successfully");
        }

        }
}
