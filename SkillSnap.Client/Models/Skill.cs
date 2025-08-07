using System.ComponentModel.DataAnnotations;

namespace SkillSnap.Client.Models
{
    public class Skill
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название обязательно")]
        [StringLength(100, ErrorMessage = "Название слишком длинное")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Уровень обязателен")]
        [StringLength(50, ErrorMessage = "Уровень слишком длинный")]
        public string? Level { get; set; }
    }
}



