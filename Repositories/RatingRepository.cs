using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using static LibraryManagementSystem.DTOs.RatingDTOs;

namespace LibraryManagementSystem.Repositories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly LibraryDbContext _context;

        public RatingRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<RatingResponseDTO> AddRatingAsync(CreateRatingDTO ratingDto, int userId)
        {
            // Check if user already rated this book
            if (await HasUserRatedBookAsync(userId, ratingDto.BookId))
            {
                throw new InvalidOperationException("You have already rated this book");
            }

            var rating = new BookRating
            {
                Rating = ratingDto.Rating,
                Review = ratingDto.Review,
                BookId = ratingDto.BookId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.BookRatings.Add(rating);
            await _context.SaveChangesAsync();

            return await GetRatingResponse(rating.RatingId);
        }

        public async Task<RatingResponseDTO?> UpdateRatingAsync(int ratingId, CreateRatingDTO ratingDto)
        {
            var rating = await _context.BookRatings.FindAsync(ratingId);
            if (rating == null) return null;

            rating.Rating = ratingDto.Rating;
            rating.Review = ratingDto.Review;
            rating.CreatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return await GetRatingResponse(ratingId);
        }

        public async Task<bool> DeleteRatingAsync(int ratingId)
        {
            var rating = await _context.BookRatings.FindAsync(ratingId);
            if (rating == null) return false;

            _context.BookRatings.Remove(rating);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<BookRatingSummaryDTO> GetBookRatingSummaryAsync(int bookId)
        {
            var book = await _context.Books
                .Include(b => b.BookRatings)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(b => b.BookId == bookId);

            if (book == null) throw new KeyNotFoundException("Book not found");

            return new BookRatingSummaryDTO
            {
                BookId = book.BookId,
                BookTitle = book.Title,
                AverageRating = book.BookRatings.Any() ?
                    book.BookRatings.Average(r => r.Rating) : 0,
                TotalRatings = book.BookRatings.Count,
                RecentReviews = book.BookRatings
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(5)
                    .Select(r => new RatingResponseDTO
                    {
                        RatingId = r.RatingId,
                        Rating = r.Rating,
                        Review = r.Review,
                        CreatedAt = r.CreatedAt,
                        BookId = r.BookId,
                        BookTitle = book.Title,
                        UserId = r.UserId,
                        Username = r.User.Username
                    })
                    .ToList()
            };
        }

        public async Task<List<RatingResponseDTO>> GetUserRatingsAsync(int userId)
        {
            return await _context.BookRatings
                .Where(r => r.UserId == userId)
                .Include(r => r.Book)
                .Include(r => r.User)
                .Select(r => new RatingResponseDTO
                {
                    RatingId = r.RatingId,
                    Rating = r.Rating,
                    Review = r.Review,
                    CreatedAt = r.CreatedAt,
                    BookId = r.BookId,
                    BookTitle = r.Book.Title,
                    UserId = r.UserId,
                    Username = r.User.Username
                })
                .ToListAsync();
        }

        public async Task<bool> HasUserRatedBookAsync(int userId, int bookId)
        {
            return await _context.BookRatings
                .AnyAsync(r => r.UserId == userId && r.BookId == bookId);
        }

        private async Task<RatingResponseDTO> GetRatingResponse(int ratingId)
        {
            return await _context.BookRatings
                .Where(r => r.RatingId == ratingId)
                .Include(r => r.Book)
                .Include(r => r.User)
                .Select(r => new RatingResponseDTO
                {
                    RatingId = r.RatingId,
                    Rating = r.Rating,
                    Review = r.Review,
                    CreatedAt = r.CreatedAt,
                    BookId = r.BookId,
                    BookTitle = r.Book.Title,
                    UserId = r.UserId,
                    Username = r.User.Username
                })
                .FirstAsync();
        }
    }
}