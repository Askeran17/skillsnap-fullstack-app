using System.ComponentModel.DataAnnotations;

public class SkillDto
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Level { get; set; }
}
