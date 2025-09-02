using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly LibraryDbContext _context;

        public BookRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .OrderByDescending(b => b.BookId)
                .Select(b => new Book
                {
                    BookId = b.BookId,
                    Title = b.Title ?? string.Empty,  // Handle NULL
                    Summary = b.Summary,  // Allowed to be NULL
                    CoverImagePath = b.CoverImagePath,
                    HardCopyAvailable = b.HardCopyAvailable,
                    SoftCopyAvailable = b.SoftCopyAvailable,
                    AudioFileAvailable = b.AudioFileAvailable,
                    TotalCopies = b.TotalCopies,
                    //CalculationAvavailableCopies = b.CalculationAvavailableCopies,
                    AvailableCopies = b.AvailableCopies,

                    // Map all other properties
                    Author = b.Author != null ? new Author
                    {
                        AuthorId = b.Author.AuthorId,
                        Name = b.Author.Name ?? string.Empty
                    } : null,
                    Category = b.Category != null ? new Category
                    {
                        CategoryId = b.Category.CategoryId,
                        Name = b.Category.Name ?? string.Empty
                    } : null,
                    // ... other properties
                })
                //.ToListAsync();
            .ToListAsync();
        }

        public async Task<PaginatedResponseDTO<Book>> GetBooksPaginatedAsync(PaginationRequestDTO pagination)
        {
            var query = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .OrderByDescending(b => b.BookId)
                .Select(b => new Book
                {
                    BookId = b.BookId,
                    Title = b.Title ?? string.Empty,
                    Summary = b.Summary,
                    CoverImagePath = b.CoverImagePath,
                    HardCopyAvailable = b.HardCopyAvailable,
                    SoftCopyAvailable = b.SoftCopyAvailable,
                    AudioFileAvailable = b.AudioFileAvailable,
                    TotalCopies = b.TotalCopies,
                    AvailableCopies = b.AvailableCopies,
                    Author = b.Author != null ? new Author
                    {
                        AuthorId = b.Author.AuthorId,
                        Name = b.Author.Name ?? string.Empty
                    } : null,
                    Category = b.Category != null ? new Category
                    {
                        CategoryId = b.Category.CategoryId,
                        Name = b.Category.Name ?? string.Empty
                    } : null,
                });

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PaginatedResponseDTO<Book>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }


        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.BookId == id);
        }

        public async Task AddBookAsync(Book book)
        {
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBookAsync(Book book)
        {
            _context.Entry(book).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBookAsync(int id)
        {
            var book = await GetBookByIdAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> BookExistsAsync(int id)
        {
            return await _context.Books.AnyAsync(b => b.BookId == id);
        }

    }
}
