namespace ContentPlatform.API.Models.DTO;

// For fetching recent content for the dashboard
public class RecentContentDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public ContentStatus Status { get; set; }
    public string Data { get; set; } = string.Empty;
}

// For the content generation request
public class CreateContentRequestDto
{
    public string Prompt { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "text" or "image"
    public Guid? ProjectId { get; set; } // Optional: associate with a project
}