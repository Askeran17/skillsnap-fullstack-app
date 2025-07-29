using System.Net.Http;
using System.Net.Http.Json;
using SkillSnap.Client.Models;

namespace SkillSnap.Client.Services
{
    public class SkillService
    {
        private readonly HttpClient _http;

        public SkillService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Skill>> GetSkillsAsync()
        {
            return await _http.GetFromJsonAsync<List<Skill>>("https://localhost:7134/api/skills");
        }

        public async Task AddSkillAsync(Skill newSkill)
        {
            var response = await _http.PostAsJsonAsync("https://localhost:7134/api/skills", newSkill);
            response.EnsureSuccessStatusCode();
        }
    }
}
