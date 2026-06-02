
namespace ContentPlatform.API.Services.AI;

public interface IAIGenerationService
{
    Task<string> GenerateTextAsync(string prompt);
    Task<string> GenerateImageAsync(string prompt);
}