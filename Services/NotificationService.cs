using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Repositories;

namespace LibraryManagementSystem.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task NotifyUserAboutApproval(int userId, string bookTitle)
        {
            var notification = new CreateNotificationDTO
            {
                UserId = userId,
                Title = "Borrow Request Approved",
                Message = $"Your borrow request for '{bookTitle}' has been approved.",
                NotificationType = "BorrowApproved"
            };

            await _notificationRepository.CreateNotificationAsync(notification);
        }

        public async Task NotifyUserAboutRejection(int userId, string bookTitle, string reason)
        {
            var message = $"Your borrow request for '{bookTitle}' has been rejected.";
            if (!string.IsNullOrEmpty(reason))
            {
                message += $" Reason: {reason}";
            }

            var notification = new CreateNotificationDTO
            {
                UserId = userId,
                Title = "Borrow Request Rejected",
                Message = message,
                NotificationType = "BorrowRejected"
            };

            await _notificationRepository.CreateNotificationAsync(notification);
        }

        public async Task NotifyUsersAboutApproval(List<int> userIds, List<string> bookTitles)
        {
            if (userIds.Count != bookTitles.Count)
                throw new ArgumentException("User IDs and Book Titles must have the same count");

            var notifications = new List<CreateNotificationDTO>();

            for (int i = 0; i < userIds.Count; i++)
            {
                notifications.Add(new CreateNotificationDTO
                {
                    UserId = userIds[i],
                    Title = "Borrow Request Approved",
                    Message = $"Your borrow request for '{bookTitles[i]}' has been approved.",
                    NotificationType = "BorrowApproved"
                });
            }

            await _notificationRepository.CreateBulkNotificationsAsync(notifications);
        }

        public async Task NotifyUsersAboutRejection(List<int> userIds, List<string> bookTitles, string reason)
        {
            if (userIds.Count != bookTitles.Count)
                throw new ArgumentException("User IDs and Book Titles must have the same count");

            var notifications = new List<CreateNotificationDTO>();

            for (int i = 0; i < userIds.Count; i++)
            {
                var message = $"Your borrow request for '{bookTitles[i]}' has been rejected.";
                if (!string.IsNullOrEmpty(reason))
                {
                    message += $" Reason: {reason}";
                }

                notifications.Add(new CreateNotificationDTO
                {
                    UserId = userIds[i],
                    Title = "Borrow Request Rejected",
                    Message = message,
                    NotificationType = "BorrowRejected"
                });
            }

            await _notificationRepository.CreateBulkNotificationsAsync(notifications);
        }
    }
}
