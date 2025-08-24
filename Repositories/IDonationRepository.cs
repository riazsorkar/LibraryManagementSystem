using LibraryManagementSystem.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Repositories
{
    public interface IDonationRepository
    {
        Task<DonationRequestDTO> CreateDonationRequestAsync(CreateDonationRequestDTO request, int userId);
        Task<List<DonationRequestDTO>> GetUserDonationsAsync(int userId);
        Task<List<DonationRequestDTO>> GetAllDonationsAsync();
        Task<List<DonationRequestDTO>> GetDonationsByStatusAsync(string status);
        Task<bool> UpdateDonationStatusAsync(int donationId, UpdateDonationStatusDTO updateDto);
    }
}