using ContentPlatform.API.Models;

namespace ContentPlatform.API.Data;

public class ProjectSupabaseRepository : IProjectSupabaseRepository
{
    public Task<Project?> GetProjectByIdAsync(Guid projectId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Content>> GetProjectContentAsync(string userId, Guid projectId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Project>> GetProjectsByUserAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Project>> GetPublicProjectsAsync()
    {
        throw new NotImplementedException();
    }

    public Task InsertProjectAsync(Project project)
    {
        throw new NotImplementedException();
    }
}