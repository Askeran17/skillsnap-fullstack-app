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
    // 🔍 Выведем полученные данные
    Console.WriteLine("Получено DTO:");
    Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(dto));

    // 📣 Проверим валидацию и выведем ошибки
    if (!ModelState.IsValid)
    {
        foreach (var kvp in ModelState)
        {
            foreach (var error in kvp.Value.Errors)
            {
                Console.WriteLine($"Ошибка в поле {kvp.Key}: {error.ErrorMessage}");
            }
        }

        return BadRequest(ModelState); // отправим ошибки обратно клиенту
    }

    // 💡 Логика создания пользователя
    var existingUser = await _userManager.FindByEmailAsync(dto.Email);
    if (existingUser != null)
        return BadRequest("Пользователь с таким Email уже существует.");

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
                return Ok(new { token });
            }

            return Unauthorized("Неверный email или пароль.");
        }

        [HttpPost("logout")]
public async Task<IActionResult> Logout()
{
    await _signInManager.SignOutAsync(); // работает с Cookie Auth
    return Ok("Вы вышли из системы.");
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


