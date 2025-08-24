// Repositories/FeaturedBookRepository.cs
using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Repositories
{
    public class FeaturedBookRepository : IFeaturedBookRepository
    {
        private readonly LibraryDbContext _context;

        public FeaturedBookRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<List<FeaturedBookDTO>> GetActiveFeaturedBooksAsync()
        {
            try
            {
                return await _context.FeaturedBooks
                    .Where(fb => fb.IsActive)
                    .Include(fb => fb.Book)
                        .ThenInclude(b => b.Author)
                    .Include(fb => fb.Book)
                        .ThenInclude(b => b.Category)
                    .OrderByDescending(fb => fb.FeaturedDate)
                    .Select(fb => new FeaturedBookDTO
                    {
                        FeaturedBookId = fb.FeaturedBookId,
                        BookId = fb.Book.BookId,
                        Title = fb.Book.Title ?? string.Empty,
                        Summary = fb.Book.Summary ?? string.Empty,
                        CoverImagePath = fb.Book.CoverImagePath ?? string.Empty,
                        HardCopyAvailable = fb.Book.HardCopyAvailable,
                        SoftCopyAvailable = fb.Book.SoftCopyAvailable ?? string.Empty,
                        AudioFileAvailable = fb.Book.AudioFileAvailable ?? string.Empty,
                        TotalCopies = fb.Book.TotalCopies,
                        AvailableCopies = fb.Book.AvailableCopies,
                        AuthorName = fb.Book.Author != null ? fb.Book.Author.Name ?? string.Empty : string.Empty,
                        CategoryName = fb.Book.Category != null ? fb.Book.Category.Name ?? string.Empty : string.Empty,
                        FeaturedDate = fb.FeaturedDate
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving featured books: {ex.Message}", ex);
            }
        }

        public async Task<bool> SetFeaturedBookAsync(int bookId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Check if book exists
                var book = await _context.Books.FindAsync(bookId);
                if (book == null)
                {
                    return false;
                }

                // Check if book is already featured and active
                var existingFeatured = await _context.FeaturedBooks
                    .FirstOrDefaultAsync(fb => fb.BookId == bookId && fb.IsActive);

                if (existingFeatured != null)
                {
                    return true; // Already featured
                }

                // Create new featured book entry
                var featuredBook = new FeaturedBook
                {
                    BookId = bookId,
                    FeaturedDate = DateTime.UtcNow,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.FeaturedBooks.Add(featuredBook);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error setting featured book: {ex.Message}", ex);
            }
        }

        public async Task<bool> RemoveFeaturedBookAsync(int featuredBookId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var featuredBook = await _context.FeaturedBooks.FindAsync(featuredBookId);
                if (featuredBook == null)
                {
                    return false;
                }

                featuredBook.IsActive = false;
                featuredBook.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error removing featured book: {ex.Message}", ex);
            }
        }

        public async Task<bool> IsBookFeaturedAsync(int bookId)
        {
            try
            {
                return await _context.FeaturedBooks
                    .AnyAsync(fb => fb.BookId == bookId && fb.IsActive);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking if book is featured: {ex.Message}", ex);
            }
        }
    }
}