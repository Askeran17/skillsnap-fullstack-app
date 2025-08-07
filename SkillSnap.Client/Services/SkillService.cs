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

        public async Task<List<SkillDto>> GetSkillsAsync()
{
    var result = await _http.GetFromJsonAsync<List<SkillDto>>("api/skills");
    return result ?? new();
}


        public async Task AddSkillAsync(SkillDto newSkill)
        {
            var response = await _http.PostAsJsonAsync("api/skills", newSkill);
            response.EnsureSuccessStatusCode();
        }
    }
}
