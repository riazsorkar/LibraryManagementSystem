using System.Security.Claims;
using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static LibraryManagementSystem.DTOs.BookDTOs;

namespace LibraryManagementSystem.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowsController : ControllerBase
    {
        private readonly IBookBorrowRepository _borrowRepository;
        private readonly LibraryDbContext _context;

        public BorrowsController(
            IBookBorrowRepository borrowRepository,
            LibraryDbContext context)
        {
            _borrowRepository = borrowRepository;
            _context = context;
        }

        [HttpGet("my-borrows")]
        public async Task<IActionResult> GetUserBorrows()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var borrows = await _borrowRepository.GetUserAllBorrowsAsync(userId);
                return Ok(borrows);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error retrieving borrow history", Error = ex.Message });
            }
        }

        [HttpGet("my-borrows/{status}")]
        public async Task<IActionResult> GetUserBorrowsByStatus(string status)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var borrows = await _borrowRepository.GetUserAllBorrowsAsync(userId);

                var filteredBorrows = borrows.Where(b => b.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
                return Ok(filteredBorrows);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error retrieving {status} borrows", Error = ex.Message });
            }
        }

        [HttpPost("borrow")]
        public async Task<IActionResult> BorrowBook([FromBody] BorrowRequestDTO request)
        {
            try
            {
                // Get user ID from token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized(new { Message = "Invalid token - missing user ID" });

                if (!int.TryParse(userIdClaim.Value, out int userId))
                    return Unauthorized(new { Message = "Invalid user ID format" });

                // Verify user exists and has borrowing privileges
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return NotFound(new { Message = "User not found" });

                // Proceed with borrowing request
                var borrowRequest = new BorrowRequestDTO
                {
                    BookId = request.BookId,
                    DueDate = request.DueDate,
                    UserId = userId
                };

                var borrow = await _borrowRepository.BorrowBookAsync(borrowRequest);

                return Ok(new
                {
                    Message = "Borrow request submitted for approval",
                    BorrowId = borrow.BorrowId,
                    Status = borrow.Status,
                    DueDate = borrow.DueDate.ToString("yyyy-MM-dd")
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error processing borrow request", Error = ex.Message });
            }
        }

        [HttpPost("extend")]
        public async Task<IActionResult> ExtendBorrow([FromBody] ExtensionRequestDTO request)
        {
            try
            {
                // Get user ID from token
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                // Verify the borrow record belongs to this user
                var borrow = await _context.BookBorrows
                    .FirstOrDefaultAsync(b => b.BorrowId == request.BorrowId && b.UserId == userId);

                if (borrow == null)
                {
                    return NotFound(new { Message = "Borrow record not found or doesn't belong to you" });
                }

                if (borrow.Status != "Borrowed")
                {
                    return BadRequest(new { Message = "Only borrowed books can be extended" });
                }

                var success = await _borrowRepository.ExtendBorrowAsync(
                    request.BorrowId,
                    request.NewDueDate);

                if (!success)
                {
                    return BadRequest(new { Message = "Unable to extend borrow period" });
                }

                return Ok(new
                {
                    Message = "Due date extended successfully",
                    NewDueDate = request.NewDueDate.ToString("yyyy-MM-dd")
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error extending due date", Error = ex.Message });
            }
        }


        [HttpPost("return/{borrowId}")]
        public async Task<IActionResult> ReturnBook(int borrowId)
        {
            try
            {
                // Get user ID from token
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                // Verify the borrow record belongs to this user
                var borrow = await _context.BookBorrows
                    .FirstOrDefaultAsync(b => b.BorrowId == borrowId && b.UserId == userId);

                if (borrow == null)
                {
                    return NotFound(new { Message = "Borrow record not found or doesn't belong to you" });
                }

                if (borrow.Status != "Borrowed")
                {
                    return BadRequest(new { Message = "Only borrowed books can be returned" });
                }

                await _borrowRepository.ReturnBookAsync(borrowId);
                return Ok(new { Message = "Book returned successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error returning book", Error = ex.Message });
            }
        }

        [HttpGet("borrow-status/{borrowId}")]
        public async Task<IActionResult> GetBorrowStatus(int borrowId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var borrow = await _context.BookBorrows
                    .Where(b => b.BorrowId == borrowId && b.UserId == userId)
                    .Select(b => new
                    {
                        b.BorrowId,
                        b.BookId,
                        BookTitle = b.Book.Title,
                        b.Status,
                        b.BorrowDate,
                        b.DueDate,
                        b.ReturnDate,
                        b.ApprovalDate,
                        CanExtend = b.Status == "Borrowed" &&
                                   b.DueDate > DateTime.UtcNow &&
                                   b.ExtensionCount < 2
                    })
                    .FirstOrDefaultAsync();

                if (borrow == null)
                {
                    return NotFound(new { Message = "Borrow record not found or doesn't belong to you" });
                }

                return Ok(borrow);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error retrieving borrow status", Error = ex.Message });
            }
        }

        [HttpPost("cancel/{borrowId}")]
        public async Task<IActionResult> CancelBorrowRequest(int borrowId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var borrow = await _context.BookBorrows
                    .FirstOrDefaultAsync(b => b.BorrowId == borrowId && b.UserId == userId);

                if (borrow == null)
                {
                    return NotFound(new { Message = "Borrow record not found or doesn't belong to you" });
                }

                if (borrow.Status != "Pending")
                {
                    return BadRequest(new { Message = "Only pending requests can be canceled" });
                }

                _context.BookBorrows.Remove(borrow);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Borrow request canceled successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error canceling borrow request", Error = ex.Message });
            }
        }
    }
}