using System.Net.Http;
using System.Net.Http.Json;
using SkillSnap.Client.Models;

namespace SkillSnap.Client.Services
{
    public class ProjectService
    {
        private readonly HttpClient _http;

        public ProjectService(HttpClient httpClient)
{
    _http = httpClient;
}


        public async Task<List<Project>> GetProjectsAsync()
        {
            var result = await _http.GetFromJsonAsync<List<Project>>("api/projects");
            return result ?? new();
        }

        public async Task AddProjectAsync(Project newProject)
        {
            var response = await _http.PostAsJsonAsync("api/projects", newProject);
            response.EnsureSuccessStatusCode();
        }
    }
}

