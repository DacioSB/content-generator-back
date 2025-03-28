using ContentPlatform.API.Data;
using ContentPlatform.API.Models;

namespace ContentPlatform.API.Services;
    public class ProjectService : IProjectService
{
    private readonly IProjectSupabaseRepository _repository;
    
    public ProjectService(IProjectSupabaseRepository repository)
    {
        _repository = repository;
    }

    public async Task<Project> CreateProjectAsync(string userId, CreateProjectDto dto)
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = dto.Name,
            IsPublic = dto.IsPublic,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            DeletedAt = null
        };
        
        await _repository.InsertProjectAsync(project);
        return project;
    }

    public async Task<IEnumerable<Project>> GetUserProjectsAsync(string userId) => 
        await _repository.GetProjectsByUserAsync(userId);

    public async Task<IEnumerable<Project>> GetPublicProjectsAsync() => 
        await _repository.GetPublicProjectsAsync();

    public async Task<Project?> GetProjectByIdAsync(Guid projectId) => 
        await _repository.GetProjectByIdAsync(projectId);

    public async Task<IEnumerable<Content>> GetProjectContentAsync(string userId, Guid projectId) => 
        await _repository.GetProjectContentAsync(userId, projectId);
}