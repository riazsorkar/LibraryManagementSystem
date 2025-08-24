using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTOs
{
    public class NotificationDTO
    {
        public int NotificationId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string NotificationType { get; set; }
    }

    public class CreateNotificationDTO
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Message { get; set; }

        public string NotificationType { get; set; }
        public int? RelatedEntityId { get; set; }
    }

    public class MarkAsReadDTO
    {
        [Required]
        public List<int> NotificationIds { get; set; }
    }
}
