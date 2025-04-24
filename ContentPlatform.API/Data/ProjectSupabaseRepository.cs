using ContentPlatform.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ContentPlatform.API.Data;

public class ProjectSupabaseRepository : IProjectSupabaseRepository
{
    private readonly ApplicationDbContext _context;

    public ProjectSupabaseRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public Task<Project?> GetProjectByIdAsync(Guid projectId)
    {
        return await _db.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
    }

    public Task<IEnumerable<Content>> GetProjectContentAsync(string userId, Guid projectId)
    {
        return await _db.Contents.Where(c => c.UserId == userId && c.ProjectId == projectId).ToListAsync();
    }

    public Task<IEnumerable<Project>> GetProjectsByUserAsync(string userId)
    {
        return await _db.Projects.Where(p => p.UserId == userId).ToListAsync();
    }

    public Task<IEnumerable<Project>> GetPublicProjectsAsync()
    {
        return await _db.Projects.Where(p => p.IsPublic).ToListAsync();
    }

    public Task InsertProjectAsync(Project project)
    {
        _db.Projects.Add(project);
        await _db.SaveChangesAsync();

        return Task.CompletedTask;
    }
}