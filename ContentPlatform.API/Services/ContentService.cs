using ContentPlatform.API.Data;
using ContentPlatform.API.Models;
using ContentPlatform.API.Models.DTO;

namespace ContentPlatform.API.Services;

public class ContentService : IContentService
{
    private readonly IContentRepository _contentRepository;

    public ContentService(IContentRepository contentRepository)
    {
        _contentRepository = contentRepository;
    }

    public async Task<IEnumerable<RecentContentDto>> GetRecentContentAsync(string userId)
    {
        var content = await _contentRepository.GetRecentContentAsync(userId, 10);
        
        return content.Select(c => new RecentContentDto
        {
            Id = c.Id.ToString(),
            Title = c.Title,
            Type = c.Type.ToLower(),
            Status = c.Status,
            Date = c.CreatedAt.ToString("g")
        });
    }

    public async Task<Content> GenerateContentAsync(string userId, CreateContentRequestDto request)
    {
        // Placeholder for actual content generation
        var generatedData = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";

        var newContent = new Content
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProjectId = request.ProjectId ?? Guid.Empty,
            Title = request.Prompt.Substring(0, Math.Min(request.Prompt.Length, 50)),
            Type = request.Type,
            Status = "Completed",
            Data = generatedData,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        return await _contentRepository.AddContentAsync(newContent);
    }
}
