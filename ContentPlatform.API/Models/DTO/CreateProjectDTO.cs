namespace ContentPlatform.API.Models.DTO;

public class CreateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
}