using System.ComponentModel.DataAnnotations;

public class SkillDto
{
    public int Id { get; set; } 
    
    [Required]
    public string Name { get; set; }

    [Required]
    public string Level { get; set; }
}
