using Azure.AI.OpenAI;
using ContentPlatform.API.Services.Storage;
using OpenAI.Chat;
using OpenAI.Images;
using System.ClientModel;

namespace ContentPlatform.API.Services.AI;

public class AzureOpenAIService : IAIGenerationService
{
    private readonly ChatClient _chatClient;

    private readonly ImageClient _imageClient;

    private readonly IStorageService _storageService;

    private readonly ILogger<AzureOpenAIService> _logger;

    public AzureOpenAIService(IConfiguration configuration, IStorageService storageService, ILogger<AzureOpenAIService> logger)
    {
        _logger = logger;
        _storageService = storageService;
        
        var endpoint = configuration["AzureOpenAI:Endpoint"] 
            ?? throw new InvalidOperationException("AzureOpenAI:Endpoint is not configured");
        var apiKey = configuration["AzureOpenAI:ApiKey"] 
            ?? throw new InvalidOperationException("AzureOpenAI:ApiKey is not configured");
        var chatDeployment = configuration["AzureOpenAI:DeploymentName"] 
            ?? throw new InvalidOperationException("AzureOpenAI:DeploymentName is not configured");
        var imageDeployment = configuration["AzureOpenAI:ImageDeploymentName"] 
            ?? throw new InvalidOperationException("AzureOpenAI:ImageDeploymentName is not configured");

        var azureClient = new AzureOpenAIClient(
            new Uri(endpoint), 
            new ApiKeyCredential(apiKey)
        );
        
        _chatClient = azureClient.GetChatClient(chatDeployment);
        _imageClient = azureClient.GetImageClient(imageDeployment);
        
        _logger.LogInformation("AzureOpenAIService initialized. Chat: {Chat}, Image: {Image}", chatDeployment, imageDeployment);
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
        try
        {
            _logger.LogInformation("Generating image for prompt: {Prompt}", prompt.Substring(0, Math.Min(50, prompt.Length)));

            ClientResult<GeneratedImage> clientResult = await _imageClient.GenerateImageAsync(prompt, new ImageGenerationOptions
            {
               Size = GeneratedImageSize.W1024xH1024,
               Quality = new GeneratedImageQuality("medium"),
            });
            BinaryData imageBytes = clientResult.Value.ImageBytes;

            if (imageBytes == null || imageBytes.ToArray().Length == 0)
            {
                throw new InvalidOperationException("Failed to generate image");
            }
            var fileName = $"{Guid.NewGuid()}.png";

            string imageUrl = await _storageService.UploadImageAsync(imageBytes.ToArray(), fileName);

            _logger.LogInformation("Successfully generated and uploaded image to {Url}", imageUrl);
            
            return imageUrl;

        } catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate image: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to generate image: {ex.Message}", ex);
        }
    }
}