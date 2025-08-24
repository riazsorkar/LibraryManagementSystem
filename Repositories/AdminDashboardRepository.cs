using LibraryManagementSystem.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Repositories
{
    public interface IAdminDashboardRepository
    {
        Task<AdminDashboardDTO> GetAdminDashboardDataAsync();
    }

    public class AdminDashboardRepository : IAdminDashboardRepository
    {
        private readonly LibraryDbContext _context;

        public AdminDashboardRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<AdminDashboardDTO> GetAdminDashboardDataAsync()
        {
            var currentDate = DateTime.UtcNow;
            var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);

            return new AdminDashboardDTO
            {
                TotalBooks = await _context.Books.CountAsync(),
                AvailableBooks = await _context.Books.CountAsync(b => b.AvailableCopies > 0),
                TotalBorrowedBooks = await _context.BookBorrows.CountAsync(b => b.Status == "Borrowed"),
                OverdueBooks = await _context.BookBorrows
                    .CountAsync(b => b.Status == "Borrowed" && b.DueDate < currentDate),
                NewMembersThisMonth = await _context.Users
                    .CountAsync(u => u.CreatedAt >= firstDayOfMonth),
                PendingDonationRequests = await _context.DonationRequests
                    .CountAsync(d => d.Status == "Pending"),
                RecentBorrows = await _context.BookBorrows
                    .Include(b => b.Book)
                    .Include(b => b.User)
                    .OrderByDescending(b => b.BorrowDate)
                    .Take(5)
                    .Select(b => new RecentActivityDTO
                    {
                        BookId = b.BookId,
                        BookTitle = b.Book.Title,
                        UserName = b.User.Username,
                        BorrowDate = b.BorrowDate,
                        DueDate = b.DueDate,
                        Status = b.DueDate < currentDate && b.Status == "Borrowed"
                            ? "Overdue" : b.Status
                    })
                    .ToListAsync()
            };
        }
    }
}