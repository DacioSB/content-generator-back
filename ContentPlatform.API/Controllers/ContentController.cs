using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ContentPlatform.API.Models.DTO;
using ContentPlatform.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

[Authorize]
[ApiController]
[Route("api/content")]
public class ContentController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ContentController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("recent")]
    public async Task<ActionResult<IEnumerable<RecentContentDto>>> GetRecentContent()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var recentContent = await _context.Contents
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .Take(10)
            .Select(c => new RecentContentDto
            {
                Id = c.Id.ToString(),
                Title = c.Title,
                Type = c.Type.ToLower(),
                Status = c.Status,
                // This is a simplification. A library like Humanizer can create "2 hours ago" strings.
                Date = c.CreatedAt.ToString("g") 
            })
            .ToListAsync();

        return Ok(recentContent);
    }
    
    // We will implement the POST method for generation in the next phase.
}
