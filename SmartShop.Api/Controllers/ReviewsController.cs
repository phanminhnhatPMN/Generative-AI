using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartShop.Api.Data;
using SmartShop.Api.DTOs;
using SmartShop.Api.Models;
using SmartShop.Api.Services;

namespace SmartShop.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly SmartShopDbContext _context;
        private readonly IGeminiService _geminiService;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(SmartShopDbContext context, IGeminiService geminiService, ILogger<ReviewsController> logger)
        {
            _context = context;
            _geminiService = geminiService;
            _logger = logger;
        }

        /// <summary>
        /// Get all customer reviews
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviews()
        {
            var reviews = await _context.Reviews.OrderByDescending(r => r.CreatedAt).AsNoTracking().ToListAsync();
            return Ok(reviews);
        }

        /// <summary>
        /// Requirement 2: Submit review, analyze sentiment via Gemini structured JSON, and save to Review table.
        /// POST /api/reviews/analyze
        /// </summary>
        [HttpPost("analyze")]
        public async Task<ActionResult<Review>> AnalyzeReview([FromBody] ReviewAnalyzeRequestDto request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productExists = await _context.Products.AnyAsync(p => p.ProductId == request.ProductId, cancellationToken);
            if (!productExists)
            {
                return NotFound(new { message = $"Product with ID {request.ProductId} does not exist." });
            }

            try
            {
                _logger.LogInformation("Analyzing review for ProductId {ProductId} by {CustomerName}", request.ProductId, request.CustomerName);

                // Call Gemini to get structured JSON analysis
                var analysisResult = await _geminiService.AnalyzeReviewAsync(request.Content, request.Rating, cancellationToken);

                var review = new Review
                {
                    ProductId = request.ProductId,
                    CustomerName = request.CustomerName.Trim(),
                    Content = request.Content.Trim(),
                    Rating = request.Rating,
                    Sentiment = analysisResult.Sentiment,
                    Score = analysisResult.Score,
                    Summary = analysisResult.Summary,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync(cancellationToken);

                return CreatedAtAction(nameof(GetReviews), new { id = review.ReviewId }, review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing review for ProductId {ProductId}", request.ProductId);
                return StatusCode(500, new { message = "Failed to analyze review with Gemini AI.", error = ex.Message });
            }
        }
    }
}

