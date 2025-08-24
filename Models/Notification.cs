namespace LibraryManagementSystem.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string NotificationType { get; set; } // "BorrowApproved", "BorrowRejected", etc.
        public int? RelatedEntityId { get; set; } // e.g., BorrowId
    }
}
