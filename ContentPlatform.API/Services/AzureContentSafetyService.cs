using Azure.AI.ContentSafety;
using ContentPlatform.API.Services.AI;

namespace ContentPlatform.API.Services;

public class AzureContentSafetyService : IModerationService
{
    private readonly ContentSafetyClient _client;
    private readonly ILogger<AzureContentSafetyService> _logger;

    // Severity threshold: 0 = safe, 2 = low, 4 = medium, 6 = high
    private const int SeverityTheshold = 4; 

    public AzureContentSafetyService(IConfiguration configuration, ILogger<AzureContentSafetyService> logger)
    {
        _logger = logger;
        var endpoint = configuration["AzureContentSafety:Endpoint"]
            ?? throw new InvalidOperationException("AzureContentSafety:Endpoint is not configured");
        var apiKey = configuration["AzureContentSafety:ApiKey"]
            ?? throw new InvalidOperationException("AzureContentSafety:ApiKey is not configured");
        _client = new ContentSafetyClient(new Uri(endpoint), new Azure.AzureKeyCredential(apiKey));

        _logger.LogInformation("AzureContentSafetyService initialized");
    }
    public Task<ModerationResult> ImgMod(byte[] image)
    {
        
        throw new NotImplementedException();
    }

    public async Task<ModerationResult> TextMod(string text)
    {
        try
        {
            _logger.LogInformation("Moderating text ({Length} chars)", text.Length);

            var request = new AnalyzeTextOptions(text);
            Azure.Response<AnalyzeTextResult> response = await _client.AnalyzeTextAsync(request);
            
            var categories = new[] {
              (Name: "Hate", Score: response.Value.CategoriesAnalysis.FirstOrDefault(c => c.Category == TextCategory.Hate)?.Severity ?? 0),  
              (Name: "Violence", Score: response.Value.CategoriesAnalysis.FirstOrDefault(c => c.Category == TextCategory.Violence)?.Severity ?? 0),  
              (Name: "Sexual", Score: response.Value.CategoriesAnalysis.FirstOrDefault(c => c.Category == TextCategory.Sexual)?.Severity ?? 0),  
              (Name: "SelfHarm", Score: response.Value.CategoriesAnalysis.FirstOrDefault(c => c.Category == TextCategory.SelfHarm)?.Severity ?? 0),  
            };

            var (Name, Score) = categories.MaxBy(c => c.Score);
            bool safe = Score < SeverityTheshold;

            _logger.LogInformation(
                "Text moderation result: {Status} (max severity: {Severity} in {Category})",
                safe ? "Safe" : "Flagged", Score, Name);

            return new ModerationResult
            {
                IsSafe = safe,
                MaxSeverity = Score,
                Reason = safe ? String.Empty : $"{Name} content detected (severity: {Score})"
            };
        }
        catch (Exception ex)
        {
            
            _logger.LogError(ex, "Failed to moderate text: {Message}", ex.Message);
            return new ModerationResult { IsSafe = true, Reason = "Moderation check skipped due to error"};
        }
    }
}