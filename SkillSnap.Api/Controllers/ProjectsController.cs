using Microsoft.AspNetCore.Mvc;
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
}



