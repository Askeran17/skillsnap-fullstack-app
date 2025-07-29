using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SkillSnap.Api.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;

namespace SkillSnap.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                return Ok();
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                var token = GenerateJwt(user);
                return Ok(new { token });
            }
            return Unauthorized();
        }

        private string GenerateJwt(ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class RegisterDto { public string Email { get; set; } public string Password { get; set; } }
    public class LoginDto { public string Email { get; set; } public string Password { get; set; } }
}
