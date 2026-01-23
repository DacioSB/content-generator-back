using ContentPlatform.API.Models;
using ContentPlatform.API.Models.DTO;

namespace ContentPlatform.API.Services;

public interface IContentService
{
    Task<Content> GenerateContentAsync(string userId, CreateContentRequestDto request);
    Task<IEnumerable<RecentContentDto>> GetRecentContentAsync(string userId);
}
