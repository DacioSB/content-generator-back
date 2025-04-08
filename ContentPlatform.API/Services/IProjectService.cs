using ContentPlatform.API.Models;
using ContentPlatform.API.Models.DTO;

namespace ContentPlatform.API.Services;
    public interface IProjectService
{
    Task<Project> CreateProjectAsync(string userId, CreateProjectDto dto);
    Task<IEnumerable<Project>> GetUserProjectsAsync(string userId);
    Task<IEnumerable<Project>> GetPublicProjectsAsync();
    Task<Project?> GetProjectByIdAsync(Guid projectId);
    Task<IEnumerable<Content>> GetProjectContentAsync(string userId, Guid projectId);
}
