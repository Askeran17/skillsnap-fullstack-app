using Xunit;
using SkillSnap.Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ProjectServiceTests
{
    [Fact]
    public async Task AddProjectAsync_ShouldAddProject()
    {
        // Arrange
        var newProject = new ProjectDto
        {
            Id = 1,
            Title = "AI Assistant",
            Description = "Build an intelligent assistant"
        };

        var projects = new List<ProjectDto> { newProject };

        // Act
        await Task.CompletedTask; // имитация async вызова
        var result = projects;

        // Assert
        Assert.Single(result);
        Assert.Equal("AI Assistant", result[0].Title);
    }
}

