using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartShop.Api.Data;
using SmartShop.Api.DTOs;
using SmartShop.Api.Services;

namespace SmartShop.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssistantController : ControllerBase
    {
        private readonly SmartShopDbContext _context;
        private readonly IGeminiService _geminiService;
        private readonly IChatSessionService _sessionService;
        private readonly ILogger<AssistantController> _logger;

        public AssistantController(
            SmartShopDbContext context,
            IGeminiService geminiService,
            IChatSessionService sessionService,
            ILogger<AssistantController> logger)
        {
            _context = context;
            _geminiService = geminiService;
            _sessionService = sessionService;
            _logger = logger;
        }

        /// <summary>
        /// Requirement 3: Shopping assistant chat with database product context and session history.
        /// POST /api/assistant/chat
        /// </summary>
        [HttpPost("chat")]
        public async Task<ActionResult<ShoppingChatResponseDto>> Chat([FromBody] ShoppingChatRequestDto request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // 1. Fetch current product catalog from database to provide as RAG context
                var products = await _context.Products.AsNoTracking().ToListAsync(cancellationToken);
                var catalogBuilder = new StringBuilder();
                foreach (var p in products)
                {
                    catalogBuilder.AppendLine($"- ID: {p.ProductId} | Name: {p.ProductName} | Category: {p.Category} | Price: {p.Price:N0} VND | Features: {p.Features} | Description: {(string.IsNullOrEmpty(p.Description) ? "N/A" : p.Description)}");
                }
                var catalogContext = catalogBuilder.ToString();

                // 2. Retrieve history for this sessionId
                var history = _sessionService.GetHistory(request.SessionId);

                _logger.LogInformation("Processing chat for sessionId: {SessionId}, message: {Message}", request.SessionId, request.Message);

                // 3. Ask Gemini
                var assistantReply = await _geminiService.ShoppingAssistantChatAsync(
                    history,
                    request.Message.Trim(),
                    catalogContext,
                    cancellationToken
                );

                // 4. Update session history
                _sessionService.AddMessage(request.SessionId, "user", request.Message.Trim());
                _sessionService.AddMessage(request.SessionId, "assistant", assistantReply);

                return Ok(new ShoppingChatResponseDto
                {
                    SessionId = request.SessionId,
                    Reply = assistantReply
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing assistant chat for session {SessionId}", request.SessionId);
                return StatusCode(500, new { message = "Failed to process chat with AI Shopping Assistant.", error = ex.Message });
            }
        }

        /// <summary>
        /// Reset conversation history for a given session
        /// </summary>
        [HttpDelete("chat/{sessionId}")]
        public IActionResult ClearSession(string sessionId)
        {
            _sessionService.Clear(sessionId);
            return Ok(new { message = $"Session {sessionId} history cleared successfully." });
        }
    }
}

