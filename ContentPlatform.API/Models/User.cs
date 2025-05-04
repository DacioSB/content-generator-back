namespace ContentPlatform.API.Models;

public class User
{
    public string Id { get; set; } = default!; // Clerk User ID (string)
    public string Email { get; set; } = string.Empty;

    // Navigation
    public List<Project> Projects { get; set; } = new();
    public List<Content> Contents { get; set; } = new();
}
