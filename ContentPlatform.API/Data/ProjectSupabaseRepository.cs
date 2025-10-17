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
    public async Task<Project?> GetProjectByIdAsync(Guid projectId)
    {
        return await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
    }

    public async Task<IEnumerable<Content>> GetProjectContentAsync(string userId, Guid projectId)
    {
        return await _context.Contents.Where(c => c.UserId == userId && c.ProjectId == projectId).ToListAsync();
    }

    public async Task<IEnumerable<Project>> GetProjectsByUserAsync(string userId)
    {
        return await _context.Projects.Where(p => p.UserId == userId).ToListAsync();
    }

    public async Task<IEnumerable<Project>> GetPublicProjectsAsync()
    {
        return await _context.Projects.Where(p => p.IsPublic).ToListAsync();
    }

    public async Task InsertProjectAsync(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
    }

    public async Task<Project?> UpdateProjectAsync(Project project)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task DeleteProjectAsync(Project project)
    {
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
    }
}