using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/admin/borrows")]
    [ApiController]
    public class AdminBorrowsController : ControllerBase
    {
        private readonly IBookBorrowRepository _borrowRepository;
        private readonly LibraryDbContext _context;
        private readonly ILogger<AdminBorrowsController> _logger;

        public AdminBorrowsController(
            IBookBorrowRepository borrowRepository,
            LibraryDbContext context,
            ILogger<AdminBorrowsController> logger)
        {
            _borrowRepository = borrowRepository;
            _context = context;
            _logger = logger;
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingBorrows([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.BookBorrows
                    .Where(b => b.Status == "Pending")
                    .Include(b => b.Book)
                    .Include(b => b.User)
                    .OrderBy(b => b.BorrowDate);

                var totalCount = await query.CountAsync();
                var pendingBorrows = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(b => new
                    {
                        b.BorrowId,
                        BookId = b.BookId,
                        BookTitle = b.Book.Title,
                        UserId = b.UserId,
                        UserName = b.User.Username,
                        b.BorrowDate,
                        b.DueDate
                    })
                    .ToListAsync();

                return Ok(new
                {
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    PendingBorrows = pendingBorrows
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending borrows");
                return StatusCode(500, "Error retrieving pending borrows");
            }
        }


        [HttpGet("Borrowed")]
        public async Task<IActionResult> GetBorrowedBorrows([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.BookBorrows
                    .Where(b => b.Status == "Borrowed")
                    .Include(b => b.Book)
                    .Include(b => b.User)
                    .OrderByDescending(b => b.BorrowDate);


                var totalCount = await query.CountAsync();
                var BorrowedBorrows = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(b => new
                    {
                        b.BorrowId,
                        BookId = b.BookId,
                        BookTitle = b.Book.Title,
                        UserId = b.UserId,
                        UserName = b.User.Username,
                        b.BorrowDate,
                        b.DueDate
                    })
                    .ToListAsync();

                return Ok(new
                {
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    BorrowedBorrows = BorrowedBorrows
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Borrowed borrows");
                return StatusCode(500, "Error retrieving Borrowed borrows");
            }
        }

        [HttpPost("approve/{borrowId}")]
        public async Task<IActionResult> ApproveBorrow(int borrowId)
        {
            try
            {
                var borrow = await _context.BookBorrows
                    .Include(b => b.Book)
                    .Include(b => b.User)
                    .FirstOrDefaultAsync(b => b.BorrowId == borrowId && b.Status == "Pending");

                if (borrow == null)
                    return NotFound("Pending borrow request not found");

                if (borrow.Book.AvailableCopies <= 0)
                    return BadRequest("No available copies to approve this request");

                var result = await _borrowRepository.ApproveBorrowAsync(borrowId);

                if (result)
                {
                    return Ok(new
                    {
                        Message = "Borrow request approved successfully",
                        BorrowId = borrow.BorrowId,
                        BookTitle = borrow.Book.Title,
                        UserName = borrow.User.Username
                    });
                }

                return BadRequest("Failed to approve borrow request");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error approving borrow {borrowId}");
                return StatusCode(500, "Error approving borrow request");
            }
        }

        [HttpPost("approve-all")]
        public async Task<IActionResult> ApproveAllPendingBorrows()
        {
            try
            {
                var pendingBorrows = await _context.BookBorrows
                    .Where(b => b.Status == "Pending")
                    .Include(b => b.Book)
                    .ToListAsync();

                if (!pendingBorrows.Any())
                    return BadRequest("No pending borrows to approve");

                // Check availability for all books
                var bookGroups = pendingBorrows
                    .GroupBy(b => b.BookId)
                    .Select(g => new
                    {
                        BookId = g.Key,
                        AvailableCopies = g.First().Book.AvailableCopies,
                        RequestCount = g.Count()
                    });

                foreach (var group in bookGroups)
                {
                    if (group.AvailableCopies < group.RequestCount)
                    {
                        return BadRequest(
                            $"Cannot approve all requests. Book ID {group.BookId} has only {group.AvailableCopies} copies but {group.RequestCount} requests");
                    }
                }

                var result = await _borrowRepository.ApproveAllPendingBorrowsAsync();

                if (result)
                {
                    return Ok(new
                    {
                        Message = "All pending borrows approved successfully",
                        ApprovedCount = pendingBorrows.Count
                    });
                }

                return BadRequest("Failed to approve all borrow requests");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving all pending borrows");
                return StatusCode(500, "Error approving all borrow requests");
            }
        }

        [HttpPost("reject/{borrowId}")]
        public async Task<IActionResult> RejectBorrow(int borrowId, [FromBody] RejectRequestDTO rejectRequest)
        {
            try
            {
                var borrow = await _context.BookBorrows
                    .Include(b => b.Book)
                    .Include(b => b.User)
                    .FirstOrDefaultAsync(b => b.BorrowId == borrowId && b.Status == "Pending");

                if (borrow == null)
                    return NotFound("Pending borrow request not found");

                var result = await _borrowRepository.RejectBorrowAsync(borrowId, rejectRequest.Reason);

                if (result)
                {
                    return Ok(new
                    {
                        Message = "Borrow request rejected successfully",
                        BorrowId = borrow.BorrowId,
                        Reason = rejectRequest.Reason
                    });
                }

                return BadRequest("Failed to reject borrow request");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error rejecting borrow {borrowId}");
                return StatusCode(500, "Error rejecting borrow request");
            }
        }

        [HttpPost("reject-all")]
        public async Task<IActionResult> RejectAllPendingBorrows([FromBody] RejectRequestDTO rejectRequest)
        {
            try
            {
                var pendingBorrows = await _context.BookBorrows
                    .Where(b => b.Status == "Pending")
                    .ToListAsync();

                if (!pendingBorrows.Any())
                    return BadRequest("No pending borrows to reject");

                var result = await _borrowRepository.RejectAllPendingBorrowsAsync(rejectRequest.Reason);

                if (result)
                {
                    return Ok(new
                    {
                        Message = "All pending borrows rejected successfully",
                        RejectedCount = pendingBorrows.Count,
                        Reason = rejectRequest.Reason
                    });
                }

                return BadRequest("Failed to reject all borrow requests");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting all pending borrows");
                return StatusCode(500, "Error rejecting all borrow requests");
            }
        }

        [HttpGet("overdue")]
        public async Task<IActionResult> GetOverdueBorrows([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.BookBorrows
                    .Where(b => b.Status == "Borrowed" && b.DueDate < DateTime.UtcNow)
                    .Include(b => b.Book)
                    .Include(b => b.User)
                    .OrderBy(b => b.DueDate);

                var totalCount = await query.CountAsync();
                var overdueBorrows = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(b => new
                    {
                        b.BorrowId,
                        BookId = b.BookId,
                        BookTitle = b.Book.Title,
                        UserId = b.UserId,
                        UserName = b.User.Username,
                        b.BorrowDate,
                        b.DueDate,
                        DaysOverdue = (int)(DateTime.UtcNow - b.DueDate).TotalDays
                    })
                    .ToListAsync();

                return Ok(new
                {
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    OverdueBorrows = overdueBorrows
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving overdue borrows");
                return StatusCode(500, "Error retrieving overdue borrows");
            }
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetBorrowStats()
        {
            try
            {
                var stats = new
                {
                    TotalBorrows = await _context.BookBorrows.CountAsync(),
                    PendingRequests = await _context.BookBorrows.CountAsync(b => b.Status == "Pending"),
                    ActiveBorrows = await _context.BookBorrows.CountAsync(b => b.Status == "Borrowed"),
                    OverdueBorrows = await _context.BookBorrows
                        .CountAsync(b => b.Status == "Borrowed" && b.DueDate < DateTime.UtcNow)
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving borrow statistics");
                return StatusCode(500, "Error retrieving borrow statistics");
            }
        }
    }

    public class RejectRequestDTO
    {
        public string Reason { get; set; }
    }
}