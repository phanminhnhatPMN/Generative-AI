using System.ComponentModel.DataAnnotations;

namespace SmartShop.Api.DTOs
{
    public class ReviewAnalyzeRequestDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(64)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        [Range(1, 5)]
        public int Rating { get; set; }
    }
}

