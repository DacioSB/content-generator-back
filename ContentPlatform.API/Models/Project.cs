namespace ContentPlatform.API.Models
{
    public class Project
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = default!;
        public string Name { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public List<Content> Contents { get; set; } = new();
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
    }
}