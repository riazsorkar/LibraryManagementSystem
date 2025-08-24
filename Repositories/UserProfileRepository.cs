using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Repositories
{
    public interface IUserProfileRepository
    {
        Task<UserProfileDTO> GetUserProfileAsync(int userId);
    }

    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly LibraryDbContext _context;

        public UserProfileRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfileDTO> GetUserProfileAsync(int userId)
        {
            var user = await _context.Users
                .Where(u => u.UserId == userId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            var borrows = await _context.BookBorrows
                .Where(b => b.UserId == userId)
                .Include(b => b.Book)
                .OrderByDescending(b => b.BorrowDate)
                .ToListAsync();

            var currentDate = DateTime.UtcNow;

            return new UserProfileDTO
            {
                username = user.Username,
                TotalBorrowedBooks = borrows.Count,
                CurrentlyBorrowedBooks = borrows.Count(b => b.Status == "Borrowed"),
                ReturnedBooks = borrows.Count(b => b.Status == "Returned"),
                OverdueBooks = borrows.Count(b => b.Status == "Borrowed" && b.DueDate < currentDate),
                RecentActivity = borrows.Take(5).Select(b => new UserBookActivityDTO
                {
                    BookId = b.BookId,
                    BookTitle = b.Book.Title,
                    BorrowDate = b.BorrowDate,
                    ReturnDate = b.ReturnDate,
                    Status = b.DueDate < currentDate && b.Status == "Borrowed" ? "Overdue" : b.Status
                }).ToList()
            };
        }
    }
}