using eLibraryAPI.Data;
using eLibraryAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace eLibraryAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthModels.LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var token = generateToken(user);

                return Ok(new { token = token });
            }

            return Unauthorized();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthModels.RegisterModel model)
        {
            var user = new IdentityUser { UserName = model.Username, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                var token = generateToken(user);
                return Ok(new { Token = token,message = "User registered successfully" });
            }

            return BadRequest(result.Errors);
        }

        private async Task<string> generateToken(IdentityUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var tokenHandler = new JwtSecurityTokenHandler();

            var authClaims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

            authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                expires: DateTime.Now.AddSeconds(double.Parse(_configuration["Jwt:ExpirySeconds"]!)),
                claims: authClaims,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                SecurityAlgorithms.HmacSha256));

            var tokenValue = tokenHandler.WriteToken(token);
            return tokenValue;
        }

    }

    public class AuthModels
    {
        public class LoginModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public class RegisterModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
        }

        public class UserRole
        {
            public string Username { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
        }
    }
}
