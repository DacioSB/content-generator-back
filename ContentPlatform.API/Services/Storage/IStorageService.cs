namespace ContentPlatform.API.Services.Storage;

public interface IStorageService
{
    Task<string> UploadImageAsync(byte[] imageBytes, string fileName);
}