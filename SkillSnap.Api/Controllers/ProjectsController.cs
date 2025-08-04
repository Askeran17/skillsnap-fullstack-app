using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SkillSnap.Api.Data;
using SkillSnap.Api.Models;
using System.Diagnostics;

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
        var sw = Stopwatch.StartNew();

        if (!_cache.TryGetValue("projects", out List<Project> projects))
        {
            projects = await _context.Projects
                .AsNoTracking()
                .Include(p => p.PortfolioUser)
                .ToListAsync();

            _cache.Set("projects", projects, TimeSpan.FromMinutes(5));
            Console.WriteLine("🟡 Кэш MISS — загружено из БД");
        }
        else
        {
            Console.WriteLine("🟢 Кэш HIT — данные из памяти");
        }

        sw.Stop();
        Console.WriteLine($"⏱️ DB Load Time: {sw.ElapsedMilliseconds}ms");

        return Ok(projects);
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

        return CreatedAtAction(nameof(GetProjects), new { id = project.Id }, project);
    }




}



