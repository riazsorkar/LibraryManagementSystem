using AutoMapper;
using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Repositories
{
    public class DonationRepository : IDonationRepository
    {
        private readonly LibraryDbContext _context;
        private readonly IMapper _mapper;

        public DonationRepository(LibraryDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<DonationRequestDTO> CreateDonationRequestAsync(CreateDonationRequestDTO request, int userId)
        {
            var donation = new DonationRequest
            {
                UserId = userId,
                BookTitle = request.BookTitle,
                AuthorName = request.AuthorName ?? string.Empty,
                Reason = request.Reason ?? string.Empty,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                RequestDate = DateTime.UtcNow,
                ProcessedDate = null,
                AdminComments = string.Empty,
                BrainStationId = request.BrainStationId ?? string.Empty,
                PhoneNumber = request.PhoneNumber ?? string.Empty,
                Address = request.Address ?? string.Empty
            };

            _context.DonationRequests.Add(donation);
            await _context.SaveChangesAsync();

            // Return without querying database to avoid NULL issues
            return new DonationRequestDTO
            {
                DonationRequestId = donation.RequestId,
                BookTitle = donation.BookTitle,
                AuthorName = donation.AuthorName,
                Reason = donation.Reason,
                Status = donation.Status,
                RequestDate = donation.RequestDate ?? donation.CreatedAt,
                ProcessedDate = donation.ProcessedDate,
                AdminComments = donation.AdminComments,
                BrainStationId = donation.BrainStationId,
                PhoneNumber = donation.PhoneNumber,
                Address = donation.Address,
                UserName = await GetUserNameAsync(userId),
                UserEmail = await GetUserEmailAsync(userId)
            };
        }

        public async Task<List<DonationRequestDTO>> GetUserDonationsAsync(int userId)
        {
            var donations = await _context.DonationRequests
                .Where(d => d.UserId == userId)
                .Include(d => d.User)
                .OrderByDescending(d => d.RequestDate)
                .ToListAsync();

            return donations.Select(d => new DonationRequestDTO
            {
                DonationRequestId = d.RequestId,
                BookTitle = d.BookTitle,
                AuthorName = d.AuthorName ?? string.Empty,
                Reason = d.Reason ?? string.Empty,
                Status = d.Status ?? "Pending",
                RequestDate = d.RequestDate ?? d.CreatedAt,
                ProcessedDate = d.ProcessedDate,
                AdminComments = d.AdminComments ?? string.Empty,
                UserName = d.User?.Username ?? string.Empty,
                UserEmail = d.User?.Email ?? string.Empty,
                BrainStationId = d.BrainStationId ?? string.Empty,
                PhoneNumber = d.PhoneNumber ?? string.Empty,
                Address = d.Address ?? string.Empty
            }).ToList();
        }

        public async Task<List<DonationRequestDTO>> GetAllDonationsAsync()
        {
            var donations = await _context.DonationRequests
                .Include(d => d.User)
                .OrderByDescending(d => d.RequestDate)
                .ToListAsync();

            return donations.Select(d => new DonationRequestDTO
            {
                DonationRequestId = d.RequestId,
                BookTitle = d.BookTitle,
                AuthorName = d.AuthorName ?? string.Empty,
                Reason = d.Reason ?? string.Empty,
                Status = d.Status ?? "Pending",
                RequestDate = d.RequestDate ?? d.CreatedAt,
                ProcessedDate = d.ProcessedDate,
                AdminComments = d.AdminComments ?? string.Empty,
                UserName = d.User?.Username ?? string.Empty,
                UserEmail = d.User?.Email ?? string.Empty,
                BrainStationId = d.BrainStationId ?? string.Empty,
                PhoneNumber = d.PhoneNumber ?? string.Empty,
                Address = d.Address ?? string.Empty
            }).ToList();
        }

        public async Task<List<DonationRequestDTO>> GetDonationsByStatusAsync(string status)
        {
            var donations = await _context.DonationRequests
                .Where(d => d.Status == status)
                .Include(d => d.User)
                .OrderByDescending(d => d.RequestDate)
                .ToListAsync();

            return donations.Select(d => new DonationRequestDTO
            {
                DonationRequestId = d.RequestId,
                BookTitle = d.BookTitle,
                AuthorName = d.AuthorName ?? string.Empty,
                Reason = d.Reason ?? string.Empty,
                Status = d.Status ?? "Pending",
                RequestDate = d.RequestDate ?? d.CreatedAt,
                ProcessedDate = d.ProcessedDate,
                AdminComments = d.AdminComments ?? string.Empty,
                UserName = d.User?.Username ?? string.Empty,
                UserEmail = d.User?.Email ?? string.Empty,
                BrainStationId = d.BrainStationId ?? string.Empty,
                PhoneNumber = d.PhoneNumber ?? string.Empty,
                Address = d.Address ?? string.Empty
            }).ToList();
        }

        public async Task<bool> UpdateDonationStatusAsync(int donationId, UpdateDonationStatusDTO updateDto)
        {
            var donation = await _context.DonationRequests.FindAsync(donationId);
            if (donation == null) return false;

            donation.Status = updateDto.NewStatus;
            donation.AdminComments = updateDto.AdminComments ?? string.Empty;
            donation.ProcessedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<string> GetUserNameAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user?.Username ?? string.Empty;
        }

        private async Task<string> GetUserEmailAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user?.Email ?? string.Empty;
        }
    }
}