using ContentPlatform.API.Models;

namespace ContentPlatform.API.Data;

public interface IProjectSupabaseRepository {
    Task InsertProjectAsync(Project project);
    Task<IEnumerable<Project>> GetProjectsByUserAsync(string userId);
    Task<IEnumerable<Project>> GetPublicProjectsAsync();
    Task<Project?> GetProjectByIdAsync(Guid projectId);
    Task<IEnumerable<Content>> GetProjectContentAsync(string userId, Guid projectId);
}