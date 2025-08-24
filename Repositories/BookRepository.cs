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



        //// Repositories/BookRepository.cs
        //public async Task<List<FeaturedBookDTO>> GetFeaturedBooksAsync()
        //{
        //    return await _context.Books
        //        .Where(b => b.IsFeatured)
        //        .Include(b => b.Author)
        //        .Select(b => new FeaturedBookDTO
        //        {
        //            BookId = b.BookId,
        //            Title = b.Title,
        //            CoverImagePath = b.CoverImagePath,
        //            AuthorName = b.Author.Name,
        //            IsFeatured = b.IsFeatured
        //        })
        //        .ToListAsync();
        //}

        //public async Task<bool> SetFeaturedStatusAsync(int bookId, bool isFeatured)
        //{
        //    var book = await _context.Books.FindAsync(bookId);
        //    if (book == null) return false;

        //    book.IsFeatured = isFeatured;
        //    await _context.SaveChangesAsync();
        //    return true;
        //}

    }
}
