using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkillSnap.Api.Data;
using SkillSnap.Api.Models;
using System.Collections.Generic;
using System.Linq;

namespace SkillSnap.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeedController : ControllerBase
    {
        private readonly SkillSnapContext _context;
        private readonly ILogger<SeedController> _logger;

        public SeedController(SkillSnapContext context, ILogger<SeedController> logger)
        {
            _context = context;
            _logger = logger;
        }

        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Seed()
        {
            if (_context.PortfolioUsers.Any())
            {
                _logger.LogInformation("Seed: Data already exists — cancelled.");
                return BadRequest("Demo data already exists.");
            }

            var user = new PortfolioUser
            {
                Name = "Jordan Developer",
                Bio = "Full-stack developer passionate about learning new tech.",
                ProfileImageUrl = "https://example.com/images/jordan.png",
                Projects = new List<Project>
                {
                    new Project { Title = "Task Tracker", Description = "Manage tasks effectively" }
                    
                },
                Skills = new List<Skill>
                {
                    new Skill { Name = "C#", Level = "Advanced" },
                    new Skill { Name = "Blazor", Level = "Intermediate" }
                }
            };

            _context.PortfolioUsers.Add(user);
            _context.SaveChanges();

            _logger.LogInformation("Seed: Demo data successfully added.");

            return Ok("Sample demo data inserted.");
        }

        
        [Authorize(Roles = "Admin")]
        [HttpDelete("clear")]
        public IActionResult Clear()
        {
            var users = _context.PortfolioUsers.ToList();
            if (!users.Any())
            {
                return BadRequest("No data to clear.");
            }

            _context.PortfolioUsers.RemoveRange(users);
            _context.SaveChanges();

            _logger.LogWarning("Seed: All demo data has been deleted.");

            return Ok("Demo data cleared.");
        }
    }
}

