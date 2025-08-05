using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using SkillSnap.Api.Models; 

public class PortfolioUser
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string Bio { get; set; }
    public string ProfileImageUrl { get; set; }

    [Required]
    public string ApplicationUserId { get; set; }

    [ForeignKey("ApplicationUserId")]
    public ApplicationUser ApplicationUser { get; set; }

    public List<Project> Projects { get; set; } = new();
    public List<Skill> Skills { get; set; } = new();
}

