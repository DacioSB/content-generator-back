using Azure.AI.OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace ContentPlatform.API.Services.AI;

public class AzureOpenAIService : IAIGenerationService
{
    private readonly ChatClient _chatClient;
    private readonly ILogger<AzureOpenAIService> _logger;

    public AzureOpenAIService(IConfiguration configuration, ILogger<AzureOpenAIService> logger)
    {
        _logger = logger;
        
        var endpoint = configuration["AzureOpenAI:Endpoint"] 
            ?? throw new InvalidOperationException("AzureOpenAI:Endpoint is not configured");
        var apiKey = configuration["AzureOpenAI:ApiKey"] 
            ?? throw new InvalidOperationException("AzureOpenAI:ApiKey is not configured");
        var deploymentName = configuration["AzureOpenAI:DeploymentName"] 
            ?? throw new InvalidOperationException("AzureOpenAI:DeploymentName is not configured");

        var azureClient = new AzureOpenAIClient(
            new Uri(endpoint), 
            new ApiKeyCredential(apiKey)
        );
        
        _chatClient = azureClient.GetChatClient(deploymentName);
        
        _logger.LogInformation("AzureOpenAIService initialized with deployment: {DeploymentName}", deploymentName);
    }

    public async Task<string> GenerateTextAsync(string prompt)
    {
        try
        {
            _logger.LogInformation("Generating text for prompt: {Prompt}", prompt.Substring(0, Math.Min(50, prompt.Length)));

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("You are a helpful AI assistant that generates high-quality content based on user prompts."),
                new UserChatMessage(prompt)
            };

            // Call the API
            var completion = await _chatClient.CompleteChatAsync(messages, new ChatCompletionOptions
            {
                Temperature = 1.0f
            });

            var generatedText = completion.Value.Content[0].Text;
            
            _logger.LogInformation("Successfully generated text");
            
            return generatedText;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate text: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to generate text: {ex.Message}", ex);
        }
    }

    public async Task<string> GenerateImageAsync(string prompt)
    {
        // Image generation will be implemented in the next task
        _logger.LogWarning("Image generation called but not yet implemented");
        await Task.CompletedTask;
        throw new NotImplementedException("Image generation will be implemented in the next task");
    }
}