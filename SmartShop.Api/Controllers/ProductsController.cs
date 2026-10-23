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
    public class ProductsController : ControllerBase
    {
        private readonly SmartShopDbContext _context;
        private readonly IGeminiService _geminiService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(SmartShopDbContext context, IGeminiService geminiService, ILogger<ProductsController> logger)
        {
            _context = context;
            _geminiService = geminiService;
            _logger = logger;
        }

        /// <summary>
        /// Get all products
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _context.Products.AsNoTracking().ToListAsync();
            return Ok(products);
        }

        /// <summary>
        /// Get product by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { message = $"Product with ID {id} not found." });
            }

            return Ok(product);
        }

        /// <summary>
        /// Requirement 1: Generate description for product and save to database.
        /// POST /api/products/{id}/generatedescription
        /// </summary>
        [HttpPost("{id}/generatedescription")]
        public async Task<ActionResult<GenerateDescriptionResponseDto>> GenerateDescription(int id, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(new object[] { id }, cancellationToken);
            if (product == null)
            {
                return NotFound(new { message = $"Product with ID {id} not found." });
            }

            try
            {
                _logger.LogInformation("Generating description for product {ProductId}: {ProductName}", product.ProductId, product.ProductName);
                
                var description = await _geminiService.GenerateProductDescriptionAsync(
                    product.ProductName,
                    product.Category,
                    product.Features,
                    cancellationToken
                );

                product.Description = description;
                await _context.SaveChangesAsync(cancellationToken);

                return Ok(new GenerateDescriptionResponseDto
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Description = product.Description
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating description for product ID {ProductId}", id);
                return StatusCode(500, new { message = "Failed to generate description with Gemini AI.", error = ex.Message });
            }
        }
    }
}

