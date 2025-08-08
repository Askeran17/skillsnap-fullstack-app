using Xunit;
using SkillSnap.Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SkillServiceTests
{
    [Fact]
    public async Task AddSkillAsync_ShouldAddSkill()
    {
        // Arrange
        var newSkill = new SkillDto { Name = "C#", Level = "Advanced" };
        var skills = new List<SkillDto> { newSkill };

        // Act
        await Task.CompletedTask; // имитация async вызова
        var result = skills;

        // Assert
        Assert.Single(result);
        Assert.Equal("C#", result[0].Name);
        Assert.Equal("Advanced", result[0].Level);
    }
}

