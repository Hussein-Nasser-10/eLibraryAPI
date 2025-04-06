using eLibraryAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eLibraryAPI.Models;
namespace eLibraryAPI.Controllers
{
        [Route("api/[controller]")]
        [ApiController]
        public class UserController : ControllerBase
        {
            private readonly ApplicationDbContext _context;

            public UserController(ApplicationDbContext context)
            {
                _context = context;
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
        }
}
