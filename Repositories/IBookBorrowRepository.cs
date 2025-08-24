using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories
{
    public interface IBookBorrowRepository
    {
        Task<BookBorrow> BorrowBookAsync(BorrowRequestDTO request);
        Task ReturnBookAsync(int borrowId);
        Task<IEnumerable<BookBorrow>> GetUserBorrowsAsync(int userId);
        //Task ExtendBorrowAsync(int borrowId, DateTime newDueDate);
        Task<bool> ExtendBorrowAsync(int borrowId, DateTime newDueDate);
        Task<List<BorrowDetailsDTO>> GetUserAllBorrowsAsync(int userId);
        Task<bool> ApproveBorrowAsync(int borrowId); // New
        Task<bool> RejectBorrowAsync(int borrowId, string reason = null); // New

        Task<bool> ApproveAllPendingBorrowsAsync();
        Task<bool> RejectAllPendingBorrowsAsync(string reason = null);
        Task<List<BookBorrow>> GetAllPendingBorrowsAsync();

    }
}