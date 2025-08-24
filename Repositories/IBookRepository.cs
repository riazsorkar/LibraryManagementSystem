using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(int id);
        Task AddBookAsync(Book book);
        Task UpdateBookAsync(Book book);
        Task DeleteBookAsync(int id);
        Task<bool> BookExistsAsync(int id);
        // Repositories/IBookRepository.cs (add these methods)
        //Task<List<FeaturedBookDTO>> GetFeaturedBooksAsync();
        //Task<bool> SetFeaturedStatusAsync(int bookId, bool isFeatured);
    }
}
