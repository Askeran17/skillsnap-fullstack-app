using Xunit;
using SkillSnap.Api.Controllers;
using SkillSnap.Api.Data;
using SkillSnap.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

public class ProjectsControllerTests
{
    private SkillSnapContext GetDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<SkillSnapContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new SkillSnapContext(options);
    }

    private IMemoryCache GetMemoryCache()
    {
        return new MemoryCache(new MemoryCacheOptions());
    }

    private PortfolioUser CreateTestUser(int id, string appUserId)
    {
        return new PortfolioUser
        {
            Id = id,
            ApplicationUserId = appUserId,
            Name = "Test Name",
            Bio = "Test Bio",
            ProfileImageUrl = "https://example.com/test.jpg"
        };
    }

    private ProjectsController GetControllerWithUser(SkillSnapContext context, string userId)
    {
        var controller = new ProjectsController(context, GetMemoryCache());

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        }, "mock"));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        return controller;
    }

    [Fact]
    public async Task GetProjects_ReturnsOkWithProjects()
    {
        // Arrange
        var context = GetDbContext(nameof(GetProjects_ReturnsOkWithProjects));
        context.PortfolioUsers.Add(CreateTestUser(1, "user1"));
        context.Projects.Add(new Project
        {
            Title = "AI Portfolio",
            Description = "A showcase of AI projects",
            PortfolioUserId = 1
        });
        context.SaveChanges();

        var controller = new ProjectsController(context, GetMemoryCache());

        // Act
        var result = await controller.GetProjects();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var projects = Assert.IsAssignableFrom<IEnumerable<ProjectDto>>(okResult.Value);
        Assert.Single(projects);
    }

    [Fact]
    public async Task AddProject_ReturnsCreatedResult()
    {
        // Arrange
        var context = GetDbContext(nameof(AddProject_ReturnsCreatedResult));
        context.PortfolioUsers.Add(CreateTestUser(2, "user2"));
        context.SaveChanges();

        var controller = GetControllerWithUser(context, "user2");

        var newProject = new ProjectDto
        {
            Title = "New Project",
            Description = "Test Description"
        };

        // Act
        var result = await controller.AddProject(newProject);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedProject = Assert.IsType<Project>(createdResult.Value);
        Assert.Equal("New Project", returnedProject.Title);
    }

    [Fact]
    public async Task DeleteProject_RemovesProject()
    {
        // Arrange
        var context = GetDbContext(nameof(DeleteProject_RemovesProject));
        context.PortfolioUsers.Add(CreateTestUser(3, "user3"));
        context.Projects.Add(new Project
        {
            Id = 10,
            Title = "To Be Deleted",
            Description = "Temporary",
            PortfolioUserId = 3
        });
        context.SaveChanges();

        var controller = new ProjectsController(context, GetMemoryCache());

        // Act
        var result = await controller.DeleteProject(10);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        Assert.Empty(context.Projects);
    }
}



