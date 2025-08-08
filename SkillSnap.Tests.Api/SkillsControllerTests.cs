using Xunit;
using SkillSnap.Api.Controllers;
using SkillSnap.Api.Models;
using SkillSnap.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SkillsControllerTests
{
    private SkillSnapContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<SkillSnapContext>()
            .UseInMemoryDatabase(databaseName: "SkillSnapTestDb")
            .Options;

        return new SkillSnapContext(options);
    }

    private IMemoryCache GetMemoryCache()
    {
        return new MemoryCache(new MemoryCacheOptions());
    }

    [Fact]
    public async Task GetSkills_ReturnsOkWithSkills()
    {
        // Arrange
        var context = GetDbContext();
        context.Skills.Add(new Skill { Name = "C#", Level = "Advanced", PortfolioUserId = 1 });
        context.SaveChanges();

        var controller = new SkillsController(context, GetMemoryCache());

        // Act
        var result = await controller.GetSkills();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var skills = Assert.IsAssignableFrom<IEnumerable<SkillDto>>(okResult.Value);
        Assert.Single(skills);
    }
}



