using ContentPlatform.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ContentPlatform.API.Data;

public class ContentRepository : IContentRepository
{
    private readonly ApplicationDbContext _context;

    public ContentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Content> AddContentAsync(Content content)
    {
        _context.Contents.Add(content);
        await _context.SaveChangesAsync();
        return content;
    }

    public async Task<IEnumerable<Content>> GetRecentContentAsync(string userId, int count)
    {
        return await _context.Contents
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .Take(count)
            .ToListAsync();
    }
}
