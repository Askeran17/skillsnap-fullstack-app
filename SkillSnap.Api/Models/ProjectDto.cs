using System.ComponentModel.DataAnnotations;

namespace SkillSnap.Api.Models
{
    public class ProjectDto
    {
        public int Id { get; set; } 
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title must be under 100 characters.")]
        public string Title { get; set; }

        [StringLength(500, ErrorMessage = "Description must be under 500 characters.")]
        public string? Description { get; set; }
    }
}
