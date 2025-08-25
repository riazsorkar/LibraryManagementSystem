using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Repositories
{
    public class BookBorrowRepository : IBookBorrowRepository
    {
        private readonly LibraryDbContext _context;

        public BookBorrowRepository(LibraryDbContext context)
        {
            _context = context;
        }


        public async Task<BookBorrow> BorrowBookAsync(BorrowRequestDTO request)
        {
            // Validate due date
            if (request.DueDate <= DateTime.UtcNow)
                throw new ArgumentException("Due date must be in the future");

            // Check book availability
            var book = await _context.Books.FindAsync(request.BookId);
            if (book == null) throw new KeyNotFoundException("Book not found");
            if (book.AvailableCopies <= 0) throw new InvalidOperationException("No copies available");

            // Check user exists
            var userExists = await _context.Users.AnyAsync(u => u.UserId == request.UserId);
            if (!userExists) throw new KeyNotFoundException("User not found");

            // Check if user already has a pending request for this book
            var existingPendingRequest = await _context.BookBorrows
                .AnyAsync(b => b.UserId == request.UserId
                            && b.BookId == request.BookId
                            && b.Status == "Pending");

            if (existingPendingRequest)
                throw new InvalidOperationException("You already have a pending request for this book");

            // Create borrow record with pending status
            var borrow = new BookBorrow
            {
                BookId = request.BookId,
                UserId = request.UserId,
                BorrowDate = DateTime.UtcNow,
                DueDate = request.DueDate,
                Status = "Pending", // Initial status is pending
                ExtensionCount = 0
            };
            // Set approval date to null initially


            _context.BookBorrows.Add(borrow);
            await _context.SaveChangesAsync();

            return borrow;
        }

        public async Task ReturnBookAsync(int borrowId)
        {
            var borrow = await _context.BookBorrows
                .Include(b => b.Book)
                .FirstOrDefaultAsync(b => b.BorrowId == borrowId);

            if (borrow == null)
                throw new KeyNotFoundException("Borrow record not found");

            if (borrow.Status != "Borrowed")
                throw new InvalidOperationException("Only borrowed books can be returned");

            borrow.Book.AvailableCopies++;
            borrow.ReturnDate = DateTime.UtcNow;
            borrow.Status = "Returned";
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<BookBorrow>> GetUserBorrowsAsync(int userId)
        {
            return await _context.BookBorrows
                .Where(b => b.UserId == userId)
                .Include(b => b.Book)
                .ToListAsync();
        }

        public async Task<bool> ExtendBorrowAsync(int borrowId, DateTime newDueDate)
        {
            var borrow = await _context.BookBorrows
                .FirstOrDefaultAsync(b => b.BorrowId == borrowId);

            if (borrow == null || borrow.Status != "Borrowed")
                return false;

            var settings = await _context.SystemSettings.FirstOrDefaultAsync();
            if (settings == null)
                throw new InvalidOperationException("System settings not configured");

            // Validate new due date is after current due date
            if (newDueDate <= borrow.DueDate)
            {
                throw new ArgumentException("New due date must be after current due date");
            }

            // Check maximum extension period (e.g., 30 days max)
            var maxExtension = borrow.DueDate.AddDays(settings.MaxBorrowDuration);
            if (newDueDate > maxExtension)
            {
                throw new ArgumentException($"Cannot extend beyond {maxExtension:yyyy-MM-dd}");
            }

            // Check if already extended too many times
            if (borrow.ExtensionCount >= settings.MaxExtensionLimit) // Example: max 2 extensions
            {
                throw new InvalidOperationException("Maximum extensions reached");
            }

            borrow.DueDate = newDueDate;
            borrow.ExtensionCount++;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<BorrowDetailsDTO>> GetUserAllBorrowsAsync(int userId)
        {
            return await _context.BookBorrows
                .Where(b => b.UserId == userId)
                .Include(b => b.Book)
                .Select(b => new BorrowDetailsDTO
                {
                    BorrowId = b.BorrowId,
                    BookId = b.BookId,
                    BookTitle = b.Book.Title,
                    BookCoverImage = b.Book.CoverImagePath,
                    BorrowDate = b.BorrowDate,
                    DueDate = b.DueDate,
                    ReturnDate = b.ReturnDate,
                    Status = b.Status,
                    ExtensionsUsed = b.ExtensionCount,
                    CanBeExtended = b.Status == "Borrowed" &&
                                  b.ExtensionCount < 2 &&
                                  b.DueDate > DateTime.UtcNow,
                    ApprovalDate = b.ApprovalDate
                })
                .OrderByDescending(b => b.BorrowDate)
                .ToListAsync();
        }

        public async Task<bool> ApproveBorrowAsync(int borrowId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var borrow = await _context.BookBorrows
                    .Include(b => b.Book)
                    .FirstOrDefaultAsync(b => b.BorrowId == borrowId);

                if (borrow == null)
                    return false;

                if (borrow.Status != "Pending")
                    return false;

                if (borrow.Book.AvailableCopies <= 0)
                    return false;

                // Check if the book is still available
                var book = await _context.Books.FindAsync(borrow.BookId);
                if (book == null || book.AvailableCopies <= 0)
                    return false;

                borrow.Status = "Borrowed";
                borrow.ApprovalDate = DateTime.UtcNow;
                book.AvailableCopies--;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> RejectBorrowAsync(int borrowId, string reason = null)
        {
            var borrow = await _context.BookBorrows
                .FirstOrDefaultAsync(b => b.BorrowId == borrowId);

            if (borrow == null || borrow.Status != "Pending")
                return false;

            borrow.Status = "Rejected";
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<BookBorrow>> GetPendingBorrowsAsync()
        {
            return await _context.BookBorrows
                .Where(b => b.Status == "Pending")
                .Include(b => b.Book)
                .Include(b => b.User)
                .OrderBy(b => b.BorrowDate)
                .ToListAsync();
        }

        public async Task<List<BookBorrow>> GetOverdueBorrowsAsync()
        {
            return await _context.BookBorrows
                .Where(b => b.Status == "Borrowed" && b.DueDate < DateTime.UtcNow)
                .Include(b => b.Book)
                .Include(b => b.User)
                .OrderBy(b => b.DueDate)
                .ToListAsync();
        }

        public async Task<bool> ApproveAllPendingBorrowsAsync()
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Get all pending borrows with book information
                var pendingBorrows = await _context.BookBorrows
                    .Include(b => b.Book)
                    .Where(b => b.Status == "Pending")
                    .ToListAsync();

                // Group by book ID to check availability in bulk
                var bookGroups = pendingBorrows
                    .GroupBy(b => b.BookId)
                    .Select(g => new
                    {
                        BookId = g.Key,
                        Book = g.First().Book,
                        RequestCount = g.Count()
                    });

                // Verify availability for all books
                foreach (var group in bookGroups)
                {
                    if (group.Book.AvailableCopies < group.RequestCount)
                    {
                        throw new InvalidOperationException(
                            $"Not enough copies available for book ID {group.BookId}. " +
                            $"Available: {group.Book.AvailableCopies}, Requested: {group.RequestCount}");
                    }
                }

                // Process approvals
                foreach (var borrow in pendingBorrows)
                {
                    borrow.Status = "Borrowed";
                    borrow.ApprovalDate = DateTime.UtcNow;
                    borrow.Book.AvailableCopies--;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Failed to approve all pending borrows: " + ex.Message, ex);
            }
        }

        public async Task<bool> RejectAllPendingBorrowsAsync(string reason = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var pendingBorrows = await _context.BookBorrows
                    .Where(b => b.Status == "Pending")
                    .ToListAsync();

                foreach (var borrow in pendingBorrows)
                {
                    borrow.Status = "Rejected";
                    // You could add a reason field to the BookBorrow model if needed
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Failed to reject all pending borrows: " + ex.Message, ex);
            }
        }

        public async Task<List<BookBorrow>> GetAllPendingBorrowsAsync()
        {
            return await _context.BookBorrows
                .Where(b => b.Status == "Pending")
                .Include(b => b.Book)
                .Include(b => b.User)
                .OrderBy(b => b.BorrowDate)
                .ToListAsync();
        }

        public async Task<int> GetUserBorrowedBooksCountAsync(int userId)
        {
            return await _context.BookBorrows
                .CountAsync(b => b.UserId == userId && b.Status == "Borrowed");
        }
    }
}