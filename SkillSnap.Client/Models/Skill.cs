using System.ComponentModel.DataAnnotations;

namespace SkillSnap.Client.Models
{
    public class Skill
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name is too long")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Level is required")]
        [StringLength(50, ErrorMessage = "Level is too long")]
        public string? Level { get; set; }
    }
}



