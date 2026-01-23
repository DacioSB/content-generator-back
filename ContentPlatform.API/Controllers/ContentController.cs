using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ContentPlatform.API.Models.DTO;
using ContentPlatform.API.Services;

[Authorize]
[ApiController]
[Route("api/content")]
public class ContentController : ControllerBase
{
    private readonly IContentService _contentService;

    public ContentController(IContentService contentService)
    {
        _contentService = contentService;
    }

    [HttpGet("recent")]
    public async Task<ActionResult<IEnumerable<RecentContentDto>>> GetRecentContent()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var recentContent = await _contentService.GetRecentContentAsync(userId);
        return Ok(recentContent);
    }
    
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateContent([FromBody] CreateContentRequestDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var newContent = await _contentService.GenerateContentAsync(userId, request);

        var dto = new RecentContentDto
        {
            Id = newContent.Id.ToString(),
            Title = newContent.Title,
            Type = newContent.Type.ToLower(),
            Status = newContent.Status,
            Date = newContent.CreatedAt.ToString("g")
        };
        
        return CreatedAtAction(nameof(GetRecentContent), new { id = newContent.Id }, dto);
    }
}
