// SystemSettingsService.cs
//using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services
{
    public class SystemSettingsService : ISystemSettingsService
    {
        private readonly LibraryDbContext _context;

        public SystemSettingsService(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<SystemSettings> GetSettingsAsync()
        {
            var settings = await _context.SystemSettings.FirstOrDefaultAsync();

            // If no settings exist, create default ones
            if (settings == null)
            {
                settings = new SystemSettings();
                _context.SystemSettings.Add(settings);
                await _context.SaveChangesAsync();
            }

            return settings;
        }

        public async Task<SystemSettings> UpdateSettingsAsync(SystemSettingsUpdateDto updateDto)
        {
            var settings = await GetSettingsAsync();

            settings.MaxBorrowDuration = updateDto.MaxBorrowDuration;
            settings.MaxBorrowLimit = updateDto.MaxBorrowLimit;
            settings.MaxExtensionLimit = updateDto.MaxExtensionLimit;
            settings.MaxBookingDuration = updateDto.MaxBookingDuration;
            settings.MaxBookingLimit = updateDto.MaxBookingLimit;
            settings.UpdatedAt = DateTime.UtcNow;

            _context.SystemSettings.Update(settings);
            await _context.SaveChangesAsync();

            return settings;
        }
    }
}