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
                Title = p.Title,
                Description = p.Description,
                
            })
            .ToListAsync();

        _cache.Set("projects", projectDtos, TimeSpan.FromMinutes(5));
        Console.WriteLine("🟡 Кэш MISS — загружено из БД");
    }
    else
    {
        Console.WriteLine("🟢 Кэш HIT — данные из памяти");
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
            return BadRequest("Не найден связанный PortfolioUser.");

        var project = new Project
        {
            Title = dto.Title,
            Description = dto.Description,
            PortfolioUserId = portfolioUserId
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        _cache.Remove("projects"); // ✅ сбрасываем кэш

        return CreatedAtAction(nameof(GetProjects), new { id = project.Id }, project);
    }




}



