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

        public async Task DeleteProjectAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/projects/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateProjectAsync(int id, ProjectDto dto)
        {
            var response = await _http.PutAsJsonAsync($"api/projects/{id}", dto);
            response.EnsureSuccessStatusCode();
        }
    }
}




