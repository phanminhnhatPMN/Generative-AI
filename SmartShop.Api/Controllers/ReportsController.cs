using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartShop.Api.Data;
using SmartShop.Api.DTOs;

namespace SmartShop.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly SmartShopDbContext _context;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(SmartShopDbContext context, ILogger<ReportsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Requirement 4: Get review report table aggregated by product.
        /// GET /api/reports/reviews
        /// </summary>
        [HttpGet("reviews")]
        public async Task<ActionResult<IEnumerable<ReviewReportDto>>> GetReviewReport()
        {
            try
            {
                // Left join Products with Reviews to include products even if they have 0 reviews
                var productsWithReviews = await _context.Products
                    .Include(p => p.Reviews)
                    .OrderBy(p => p.ProductId)
                    .AsNoTracking()
                    .ToListAsync();

                var report = new List<ReviewReportDto>();
                int index = 1;

                foreach (var product in productsWithReviews)
                {
                    var reviews = product.Reviews.ToList();
                    var totalReviews = reviews.Count;
                    var positiveCount = reviews.Count(r => r.Sentiment.Equals("Positive", StringComparison.OrdinalIgnoreCase));
                    var negativeCount = reviews.Count(r => r.Sentiment.Equals("Negative", StringComparison.OrdinalIgnoreCase));
                    var neutralCount = reviews.Count(r => r.Sentiment.Equals("Neutral", StringComparison.OrdinalIgnoreCase));
                    
                    var averageRating = totalReviews > 0
                        ? Math.Round(reviews.Average(r => r.Rating), 1)
                        : 0.0;

                    report.Add(new ReviewReportDto
                    {
                        No = index++,
                        ProductName = product.ProductName,
                        TotalReviews = totalReviews,
                        Positive = positiveCount,
                        Negative = negativeCount,
                        Neutral = neutralCount,
                        AverageRating = averageRating
                    });
                }

                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating review report");
                return StatusCode(500, new { message = "Failed to generate review report.", error = ex.Message });
            }
        }
    }
}

