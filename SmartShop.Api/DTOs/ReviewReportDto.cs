namespace SmartShop.Api.DTOs
{
    public class ReviewReportDto
    {
        public int No { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int TotalReviews { get; set; }
        public int Positive { get; set; }
        public int Negative { get; set; }
        public int Neutral { get; set; }
        public double AverageRating { get; set; }
    }
}

