using System.Net.Http;
using System.Net.Http.Json;
using SkillSnap.Client.Models;

namespace SkillSnap.Client.Services
{
    public class ProjectService
    {
        private readonly HttpClient _http;

        public ProjectService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Project>> GetProjectsAsync()
        {
            return await _http.GetFromJsonAsync<List<Project>>("https://localhost:7134/api/projects");
        }

        public async Task AddProjectAsync(Project newProject)
        {
            var response = await _http.PostAsJsonAsync("https://localhost:7134/api/projects", newProject);
            response.EnsureSuccessStatusCode();
        }
    }
}
