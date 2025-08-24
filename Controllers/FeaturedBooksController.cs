// Controllers/FeaturedBooksController.cs
using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeaturedBooksController : ControllerBase
    {
        private readonly IFeaturedBookRepository _featuredBookRepository;

        public FeaturedBooksController(IFeaturedBookRepository featuredBookRepository)
        {
            _featuredBookRepository = featuredBookRepository;
        }

        // GET: api/featuredbooks
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<FeaturedBookDTO>>> GetFeaturedBooks()
        {
            try
            {
                var featuredBooks = await _featuredBookRepository.GetActiveFeaturedBooksAsync();
                return Ok(featuredBooks);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        
        // POST: api/featuredbooks
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetFeaturedBook([FromBody] SetFeaturedBookDTO setFeaturedDto)
        {
            try
            {
                if (setFeaturedDto == null || setFeaturedDto.BookId <= 0)
                {
                    return BadRequest("Invalid book ID");
                }

                var success = await _featuredBookRepository.SetFeaturedBookAsync(setFeaturedDto.BookId);

                if (!success)
                    return BadRequest("Book not found or could not be featured");

                return Ok(new { message = "Book set as featured successfully" });
            }
            catch (Exception ex)
            {
                // Log the detailed error for debugging
                Console.WriteLine($"Error in SetFeaturedBook: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/featuredbooks/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveFeaturedBook(int id)
        {
            try
            {
                var success = await _featuredBookRepository.RemoveFeaturedBookAsync(id);
                if (!success) return NotFound("Featured book entry not found");

                return Ok(new { message = "Book removed from featured list successfully" });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/featuredbooks/check/{bookId}
        [HttpGet("check/{bookId}")]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> IsBookFeatured(int bookId)
        {
            try
            {
                var isFeatured = await _featuredBookRepository.IsBookFeaturedAsync(bookId);
                return Ok(isFeatured);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}