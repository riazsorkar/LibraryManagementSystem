using AutoMapper;
using Microsoft.EntityFrameworkCore;
using static LibraryManagementSystem.DTOs.BookDTOs;

namespace LibraryManagementSystem.Services
{
    public interface IRecommendationService
    {
        Task<List<BookResponseDTO>> GetPopularBooksAsync(int count = 5);
        Task<List<BookResponseDTO>> GetRecommendedForUserAsync(int userId, int count = 5);
        Task<List<BookResponseDTO>> GetSimilarBooksAsync(int bookId, int count = 5);
    }

    public class RecommendationService : IRecommendationService
    {
        private readonly LibraryDbContext _context;
        private readonly IMapper _mapper;

        public RecommendationService(LibraryDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Get most frequently borrowed books
        public async Task<List<BookResponseDTO>> GetPopularBooksAsync(int count = 5)
        {
            var popularBooks = await _context.Books
                .Include(b => b.Author)  // Include author
                .Include(b => b.Category) // Include category
                .OrderByDescending(b => b.BookBorrows.Count)
                .Take(count)
                .ToListAsync();

            return _mapper.Map<List<BookResponseDTO>>(popularBooks);
        }

        public async Task<List<BookResponseDTO>> GetRecommendedForUserAsync(int userId, int count = 5)
        {
            var favoriteCategory = await _context.BookBorrows
                .Where(b => b.UserId == userId)
                .Include(b => b.Book)
                    .ThenInclude(b => b.Category)
                .GroupBy(b => b.Book.CategoryId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefaultAsync();

            if (favoriteCategory == null)
                return await GetPopularBooksAsync(count);

            var recommendedBooks = await _context.Books
                .Where(b => b.CategoryId == favoriteCategory)
                .Include(b => b.Author)  // Include author
                .Include(b => b.Category) // Include category
                .OrderByDescending(b => b.BookBorrows.Count)
                .Take(count)
                .ToListAsync();

            return _mapper.Map<List<BookResponseDTO>>(recommendedBooks);
        }

        public async Task<List<BookResponseDTO>> GetSimilarBooksAsync(int bookId, int count = 5)
        {
            var book = await _context.Books
                .Include(b => b.Category) // Include category for comparison
                .FirstOrDefaultAsync(b => b.BookId == bookId);

            if (book == null) return new List<BookResponseDTO>();

            var similarBooks = await _context.Books
                .Where(b => b.CategoryId == book.CategoryId && b.BookId != bookId)
                .Include(b => b.Author)  // Include author
                .Include(b => b.Category) // Include category
                .OrderByDescending(b => b.BookBorrows.Count)
                .Take(count)
                .ToListAsync();

            return _mapper.Map<List<BookResponseDTO>>(similarBooks);
        }
    }
}
