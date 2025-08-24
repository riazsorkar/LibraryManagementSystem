// Repositories/IFeaturedBookRepository.cs
using LibraryManagementSystem.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Repositories
{
    public interface IFeaturedBookRepository
    {
        Task<List<FeaturedBookDTO>> GetActiveFeaturedBooksAsync();
        Task<bool> SetFeaturedBookAsync(int bookId);
        Task<bool> RemoveFeaturedBookAsync(int featuredBookId);
        Task<bool> IsBookFeaturedAsync(int bookId);
    }
}