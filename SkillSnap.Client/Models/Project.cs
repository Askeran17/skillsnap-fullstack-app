using System.ComponentModel.DataAnnotations;

namespace SkillSnap.Client.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title can’t exceed 100 characters.")]
        public string Title { get; set; }

        [StringLength(500, ErrorMessage = "Description can’t exceed 500 characters.")]
        public string Description { get; set; }
    }
}
