using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SkillSnap.Api.Data;
using SkillSnap.Api.Models;
using System.Diagnostics;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]


public class ProjectsController : ControllerBase
{
    private readonly SkillSnapContext _context;
    private readonly IMemoryCache _cache;

    public ProjectsController(SkillSnapContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpGet]
    public async Task<IActionResult> GetProjects()
    {
        if (!_cache.TryGetValue("projects", out List<ProjectDto> projectDtos))
        {
            projectDtos = await _context.Projects
                .AsNoTracking()
                .Include(p => p.PortfolioUser)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,

                })
                .ToListAsync();

            _cache.Set("projects", projectDtos, TimeSpan.FromMinutes(5));
            Console.WriteLine("🟡 Cache MISS — uploaded from DB");
        }
        else
        {
            Console.WriteLine("🟢 Cache HIT — data from memory");
        }

        return Ok(projectDtos);
    }


    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddProject([FromBody] ProjectDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var portfolioUserId = await _context.PortfolioUsers
            .Where(p => p.ApplicationUserId == userId)
            .Select(p => p.Id)
            .FirstOrDefaultAsync();

        if (portfolioUserId == 0)
            return BadRequest("There's no PortfolioUser.");

        var project = new Project
        {
            Title = dto.Title,
            Description = dto.Description,
            PortfolioUserId = portfolioUserId
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        _cache.Remove("projects");

        return CreatedAtAction(nameof(GetProjects), new { id = project.Id }, project);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectDto dto)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null) return NotFound("Project not found.");

        project.Title = dto.Title;
        project.Description = dto.Description;
        await _context.SaveChangesAsync();

        _cache.Remove("projects");
        return Ok("✅ Project updated.");
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null) return NotFound("Project not found.");

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

        _cache.Remove("projects");
        return Ok("🗑️ Project deleted.");
    }



}



