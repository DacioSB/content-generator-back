using ContentPlatform.API.Data;
using ContentPlatform.API.Models;
using ContentPlatform.API.Models.DTO;
using ContentPlatform.API.Services.AI;

namespace ContentPlatform.API.Services;

public class ContentService : IContentService
{
    private readonly IContentRepository _contentRepository;
    private readonly IAIGenerationService _aiService;
    private readonly ILogger<ContentService> _logger;

    public ContentService(
        IContentRepository contentRepository, 
        IAIGenerationService aiService,
        ILogger<ContentService> logger)
    {
        _contentRepository = contentRepository;
        _aiService = aiService;
        _logger = logger;
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
        string generatedData;

        try
        {
            if (request.Type.ToLower() == "text")
            {
                _logger.LogInformation("Generating text content for user {UserId}", userId);
                generatedData = await _aiService.GenerateTextAsync(request.Prompt);
            }
            else if (request.Type.ToLower() == "image")
            {
                _logger.LogInformation("Generating image content for user {UserId}", userId);
                generatedData = await _aiService.GenerateImageAsync(request.Prompt);
            }
            else
            {
                throw new ArgumentException($"Invalid content type: {request.Type}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate content for user {UserId}", userId);
            throw new InvalidOperationException("Content generation failed. Please try again.", ex);
        }

        var newContent = new Content
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProjectId = request.ProjectId == Guid.Empty ? null : request.ProjectId,
            Title = request.Prompt.Substring(0, Math.Min(request.Prompt.Length, 50)),
            Type = request.Type,
            Status = "Completed", // Will be set by moderation in later tasks
            Data = generatedData,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        return await _contentRepository.AddContentAsync(newContent);
    }
}
