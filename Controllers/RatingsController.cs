using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static LibraryManagementSystem.DTOs.RatingDTOs;

namespace LibraryManagementSystem.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly IRatingRepository _ratingRepository;

        public RatingsController(IRatingRepository ratingRepository)
        {
            _ratingRepository = ratingRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddRating([FromBody] CreateRatingDTO ratingDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            try
            {
                var rating = await _ratingRepository.AddRatingAsync(ratingDto, userId);
                return CreatedAtAction(nameof(GetRating), new { id = rating.RatingId }, rating);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRating(int id, [FromBody] CreateRatingDTO ratingDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var rating = await _ratingRepository.UpdateRatingAsync(id, ratingDto);

            if (rating == null) return NotFound();
            if (rating.UserId != userId) return Forbid();

            return Ok(rating);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRating(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var ratings = await _ratingRepository.GetUserRatingsAsync(userId);
            var rating = ratings.FirstOrDefault(r => r.RatingId == id);

            if (rating == null) return NotFound();

            var success = await _ratingRepository.DeleteRatingAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }

        [HttpGet("book/{bookId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBookRatings(int bookId)
        {
            try
            {
                var summary = await _ratingRepository.GetBookRatingSummaryAsync(bookId);
                return Ok(summary);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserRatings()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var ratings = await _ratingRepository.GetUserRatingsAsync(userId);
            return Ok(ratings);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRating(int id)
        {
            var ratings = await _ratingRepository.GetUserRatingsAsync(0);
            var rating = ratings.FirstOrDefault(r => r.RatingId == id);

            if (rating == null) return NotFound();
            return Ok(rating);
        }
    }
}