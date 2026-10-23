using SmartShop.Api.DTOs;

namespace SmartShop.Api.Services
{
    public interface IGeminiService
    {
        Task<string> GenerateProductDescriptionAsync(string productName, string category, string features, CancellationToken cancellationToken = default);
        Task<ReviewAnalysisResultDto> AnalyzeReviewAsync(string content, int rating, CancellationToken cancellationToken = default);
        Task<string> ShoppingAssistantChatAsync(List<(string Role, string Text)> history, string userMessage, string catalogContext, CancellationToken cancellationToken = default);
    }
}

