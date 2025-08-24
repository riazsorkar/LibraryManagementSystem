using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories
{
    public interface INotificationRepository
    {
        Task<Notification> CreateNotificationAsync(CreateNotificationDTO notificationDto);
        Task<List<Notification>> GetUserNotificationsAsync(int userId);
        Task<int> GetUnreadNotificationCountAsync(int userId);
        Task MarkNotificationsAsReadAsync(List<int> notificationIds);
        Task CreateBulkNotificationsAsync(List<CreateNotificationDTO> notificationDtos);
    }
}
