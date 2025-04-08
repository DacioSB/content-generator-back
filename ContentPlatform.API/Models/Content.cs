namespace ContentPlatform.API.Models;
public class Content
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Data { get; set; } = string.Empty; // URL to S3 bucket
    public string UserId { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}