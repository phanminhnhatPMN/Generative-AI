using System.ComponentModel.DataAnnotations;

namespace SmartShop.Api.DTOs
{
    public class ShoppingChatRequestDto
    {
        [Required]
        public string SessionId { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;
    }
}

