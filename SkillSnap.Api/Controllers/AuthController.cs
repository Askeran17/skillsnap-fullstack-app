using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SkillSnap.Api.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using System.Globalization;

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

            Console.WriteLine("Got DTO:");
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(dto));

            if (!ModelState.IsValid)
            {
                foreach (var kvp in ModelState)
                {
                    foreach (var error in kvp.Value.Errors)
                    {
                        Console.WriteLine($"Error in field {kvp.Key}: {error.ErrorMessage}");
                    }
                }

                return BadRequest(ModelState);
            }

            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return BadRequest("User with this Email already exists.");

            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email
            };

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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                var token = await GenerateJwt(user);
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? "User";

                return Ok(new
                {
                    token,
                    email = user.Email,
                    userName = user.UserName,
                    role = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(role.ToLower()),
                    userId = user.Id
                });
            }

            return Unauthorized("Invalid email or password.");
        }



        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("You have logged out.");
        }


        private async Task<string> GenerateJwt(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(
                    ClaimTypes.Role,
                    CultureInfo.InvariantCulture.TextInfo.ToTitleCase(role.ToLower())));
            }

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
}


