namespace ContentPlatform.API.Models;

public class CreateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
}