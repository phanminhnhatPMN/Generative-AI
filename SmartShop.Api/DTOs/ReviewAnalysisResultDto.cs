using System.Text.Json.Serialization;

namespace SmartShop.Api.DTOs
{
    public class ReviewAnalysisResultDto
    {
        [JsonPropertyName("sentiment")]
        public string Sentiment { get; set; } = string.Empty; // Positive | Negative | Neutral

        [JsonPropertyName("score")]
        public double Score { get; set; }

        [JsonPropertyName("summary")]
        public string Summary { get; set; } = string.Empty;
    }
}

