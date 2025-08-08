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

        
        [HttpGet]
        public async Task<IActionResult> GetSkills()
        {
            if (!_cache.TryGetValue("skills", out List<SkillDto> skillDtos))
            {
                skillDtos = await _context.Skills
                    .AsNoTracking()
                    .Select(s => new SkillDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Level = s.Level
                    })
                    .ToListAsync();

                _cache.Set("skills", skillDtos, TimeSpan.FromMinutes(5));
                Console.WriteLine("🟡 Cache MISS — uploaded from DB");
            }
            else
            {
                Console.WriteLine("🟢 Cache HIT — data from memory");
            }

            return Ok(skillDtos);
        }

        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddSkill([FromBody] SkillDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var portfolioUserId = await _context.PortfolioUsers
                .Where(p => p.ApplicationUserId == userId)
                .Select(p => p.Id)
                .FirstOrDefaultAsync();

            if (portfolioUserId == 0)
                return BadRequest("❌ No associated PortfolioUser found.");

            var skill = new Skill
            {
                Name = dto.Name,
                Level = dto.Level,
                PortfolioUserId = portfolioUserId
            };

            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();

            _cache.Remove("skills");

            return CreatedAtAction(nameof(GetSkills), new { id = skill.Id }, new SkillDto
            {
                Id = skill.Id,
                Name = skill.Name,
                Level = skill.Level
            });
        }

        
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSkill(int id, [FromBody] SkillDto dto)
        {
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null)
                return NotFound("❌ Skill not found.");

            skill.Name = dto.Name;
            skill.Level = dto.Level;
            await _context.SaveChangesAsync();

            _cache.Remove("skills");
            return Ok("✅ Skill updated.");
        }

        
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSkill(int id)
        {
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null)
                return NotFound("❌ Skill not found.");

            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();

            _cache.Remove("skills");
            return Ok("🗑️ Skill deleted.");
        }
    }
}




