using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace ContentPlatform.API.Services.Storage;

public class AzureBlobStorageService : IStorageService
{
    private readonly BlobContainerClient _containerClient;
    private readonly ILogger<AzureBlobStorageService> _logger;

    public AzureBlobStorageService(IConfiguration configuration, ILogger<AzureBlobStorageService> logger)
    {
        _logger = logger;

        var connectionString = configuration["AzureBlobStorage:ConnectionString"]
            ?? throw new InvalidOperationException("AzureBlobStorage:ConnectionString is not configured");
        var containerName = configuration["AzureBlobStorage:ContainerName"]
            ?? throw new InvalidOperationException("AzureBlobStorage:ContainerName is not configured");

        _containerClient = new BlobContainerClient(connectionString, containerName);

        _logger.LogInformation("AzureBlobStorageService initialized with container: {Container}", containerName);
    }

    public async Task<string> UploadImageAsync(byte[] imageBytes, string fileName)
    {
        try
        {
            _logger.LogInformation("Uploading image: {FileName} ({Size} bytes)", fileName, imageBytes.Length);

            var blobClient = _containerClient.GetBlobClient(fileName);

            using var stream = new MemoryStream(imageBytes);
            await blobClient.UploadAsync(stream, new BlobHttpHeaders
            {
                ContentType = "image/png"
            });

            var imageUrl = blobClient.Uri.ToString();
            _logger.LogInformation("Successfully uploaded image to: {Url}", imageUrl);
            return imageUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload image: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to upload image: {ex.Message}", ex);
        }
    }
}