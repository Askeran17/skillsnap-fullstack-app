using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSnap.Api.Data;
using SkillSnap.Api.Models;

namespace SkillSnap.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillsController : ControllerBase
    {
        private readonly SkillSnapContext _context;

        public SkillsController(SkillSnapContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Skill>>> GetSkills()
        {
            var skills = await _context.Skills
                .AsNoTracking()
                .Include(s => s.PortfolioUser)
                .ToListAsync();

            return Ok(skills);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Skill>> AddSkill([FromBody] Skill skill)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSkills), new { id = skill.Id }, skill);
        }
    }
}

