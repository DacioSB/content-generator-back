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

    private readonly IModerationService _moderationService;

    public ContentService(
        IContentRepository contentRepository, 
        IAIGenerationService aiService,
        ILogger<ContentService> logger,
        IModerationService moderationService)
    {
        _contentRepository = contentRepository;
        _aiService = aiService;
        _logger = logger;
        _moderationService = moderationService;
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
            Date = c.CreatedAt.ToString("g"),
            Data = c.Data
        });
    }

    public async Task<Content> GenerateContentAsync(string userId, CreateContentRequestDto request)
    {
        string generatedData;
        ModerationResult moderationResult;

        try
        {
            if (request.Type.ToLower() == "text")
            {
                _logger.LogInformation("Generating text content for user {UserId}", userId);
                generatedData = await _aiService.GenerateTextAsync(request.Prompt);
                moderationResult = await _moderationService.TextMod(generatedData);
            }
            else if (request.Type.ToLower() == "image")
            {
                _logger.LogInformation("Generating image content for user {UserId}", userId);
                generatedData = await _aiService.GenerateImageAsync(request.Prompt);
                moderationResult = await _moderationService.ImgMod(generatedData);
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

        if (!moderationResult.IsSafe)
        {
            _logger.LogWarning("Content flagged for user {UserId}: {Reason}", userId, moderationResult.Reason);
        }

        var newContent = new Content
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProjectId = request.ProjectId == Guid.Empty ? null : request.ProjectId,
            Title = request.Prompt.Substring(0, Math.Min(request.Prompt.Length, 50)),
            Type = request.Type,
            Status = moderationResult.IsSafe ? ContentStatus.Completed : ContentStatus.Flagged,
            Data = generatedData,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        return await _contentRepository.AddContentAsync(newContent);
    }
}
