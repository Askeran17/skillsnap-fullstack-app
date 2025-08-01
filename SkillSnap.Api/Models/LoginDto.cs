// LoginDto.cs
using System.ComponentModel.DataAnnotations;

namespace SkillSnap.Api.Models
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
