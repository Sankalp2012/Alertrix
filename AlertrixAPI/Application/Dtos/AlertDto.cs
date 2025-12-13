namespace AlertrixAPI.Application.DTOs
{
    public class AlertDto
    {
        public string Id { get; set; } = string.Empty;
        public string? UserId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
