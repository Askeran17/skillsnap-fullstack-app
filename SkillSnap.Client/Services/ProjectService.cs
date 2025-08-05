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

        public async Task<List<ProjectDto>> GetProjectsAsync()
        {
            var response = await _http.GetFromJsonAsync<List<ProjectDto>>("api/projects");
            return response ?? new List<ProjectDto>();
        }

        public async Task AddProjectAsync(ProjectDto newProject)
        {
            var response = await _http.PostAsJsonAsync("api/projects", newProject);
            response.EnsureSuccessStatusCode();
        }
    }
}



