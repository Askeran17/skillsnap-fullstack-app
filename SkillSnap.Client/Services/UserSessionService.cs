namespace SkillSnap.Client.Services
{
    public class UserSessionService
    {
        public string? UserId { get; set; }
        public string? Role { get; set; }
        public int? EditingProjectId { get; set; }

        public bool IsAdmin => Role == "Admin";
    }
}
