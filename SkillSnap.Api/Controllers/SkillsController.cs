using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SkillSnap.Api.Data;
using SkillSnap.Api.Models;
using System.Security.Claims;

namespace SkillSnap.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillsController : ControllerBase
    {
        private readonly SkillSnapContext _context;
        private readonly IMemoryCache _cache;

        public SkillsController(SkillSnapContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // 📦 Получение навыков с кэшированием
        [HttpGet]
        public async Task<IActionResult> GetSkills()
        {
            if (!_cache.TryGetValue("skills", out List<SkillDto> skillDtos))
            {
                skillDtos = await _context.Skills
                    .AsNoTracking()
                    .Select(s => new SkillDto
                    {
                        Name = s.Name,
                        Level = s.Level
                    })
                    .ToListAsync();

                _cache.Set("skills", skillDtos, TimeSpan.FromMinutes(5));
                Console.WriteLine("🟡 Кэш MISS — загружено из БД");
            }
            else
            {
                Console.WriteLine("🟢 Кэш HIT — данные из памяти");
            }

            return Ok(skillDtos);
        }

        // 🔐 Добавление навыка с авторизацией
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddSkill([FromBody] SkillDto dto)
        {
            // ✅ Проверка модели
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var portfolioUserId = await _context.PortfolioUsers
                .Where(p => p.ApplicationUserId == userId)
                .Select(p => p.Id)
                .FirstOrDefaultAsync();

            if (portfolioUserId == 0)
                return BadRequest("❌ Не найден связанный PortfolioUser.");

            var skill = new Skill
            {
                Name = dto.Name,
                Level = dto.Level,
                PortfolioUserId = portfolioUserId
            };

            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();

            _cache.Remove("skills"); // ✅ сбрасываем кэш

            return CreatedAtAction(nameof(GetSkills), new { id = skill.Id }, new SkillDto
            {
                Name = skill.Name,
                Level = skill.Level
            });
        }
    }
}



