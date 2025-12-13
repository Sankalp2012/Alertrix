using System.ComponentModel.DataAnnotations;

namespace AlertrixAPI.Application.DTOs
{
    public class AlertCreateDto
    {
        [Required]
        [StringLength(120, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        public string? UserId { get; set; } = string.Empty;
    }
}
