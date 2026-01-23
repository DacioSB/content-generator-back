using ContentPlatform.API.Models;

namespace ContentPlatform.API.Data;

public interface IContentRepository
{
    Task<Content> AddContentAsync(Content content);
    Task<IEnumerable<Content>> GetRecentContentAsync(string userId, int count);
}
