// ISystemSettingsService.cs
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services
{
    public interface ISystemSettingsService
    {
        Task<SystemSettings> GetSettingsAsync();
        Task<SystemSettings> UpdateSettingsAsync(SystemSettingsUpdateDto updateDto);
    }
}
