using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static LibraryManagementSystem.DTOs.BookDTOs;
using System.Security.Claims;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/recommendations")]
    [ApiController]
    [Authorize]
    public class RecommendationsController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;

        public RecommendationsController(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        // GET: api/recommendations/popular
        [HttpGet("popular")]
        [AllowAnonymous]
        public async Task<ActionResult<List<BookResponseDTO>>> GetPopularBooks([FromQuery] int count = 5)
        {
            var books = await _recommendationService.GetPopularBooksAsync(count);
            return Ok(books);
        }

        // GET: api/recommendations/for-user
        [HttpGet("for-user")]
        public async Task<ActionResult<List<BookResponseDTO>>> GetRecommendedForUser([FromQuery] int count = 5)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var books = await _recommendationService.GetRecommendedForUserAsync(userId, count);
            return Ok(books);
        }

        // GET: api/recommendations/similar/{bookId}
        [HttpGet("similar/{bookId}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<BookResponseDTO>>> GetSimilarBooks(int bookId, [FromQuery] int count = 5)
        {
            var books = await _recommendationService.GetSimilarBooksAsync(bookId, count);
            return Ok(books);
        }
    }
}
