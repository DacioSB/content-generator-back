namespace ContentPlatform.API.Services.AI;

public class ModerationResult
{
    public bool IsSafe {get; set;}
    public int MaxSeverity {get; set;}
    public string Reason {get; set;} = string.Empty;
}

public interface IModerationService
{
    public Task<ModerationResult> TextMod(string text);
    public Task<ModerationResult> ImgMod(byte[] image);
}