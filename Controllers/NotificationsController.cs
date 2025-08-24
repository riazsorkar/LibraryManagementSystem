using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LibraryManagementSystem.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly LibraryDbContext _context;

        public NotificationsController(
            INotificationRepository notificationRepository,
            LibraryDbContext context)
        {
            _notificationRepository = notificationRepository;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserNotifications()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var notifications = await _notificationRepository.GetUserNotificationsAsync(userId);

                var result = notifications.Select(n => new NotificationDTO
                {
                    NotificationId = n.NotificationId,
                    Title = n.Title,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt,
                    NotificationType = n.NotificationType
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error retrieving notifications", Error = ex.Message });
            }
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadNotificationCount()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var count = await _notificationRepository.GetUnreadNotificationCountAsync(userId);
                return Ok(new { UnreadCount = count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error retrieving unread count", Error = ex.Message });
            }
        }

        [HttpPost("mark-as-read")]
        public async Task<IActionResult> MarkNotificationsAsRead([FromBody] MarkAsReadDTO dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Verify all notifications belong to the user
                var userNotifications = await _context.Notifications
                    .Where(n => n.UserId == userId && dto.NotificationIds.Contains(n.NotificationId))
                    .ToListAsync();

                await _notificationRepository.MarkNotificationsAsReadAsync(
                    userNotifications.Select(n => n.NotificationId).ToList());

                return Ok(new { Message = "Notifications marked as read" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error marking notifications as read", Error = ex.Message });
            }
        }

        [HttpDelete("{notificationId}")]
        public async Task<IActionResult> DeleteNotification(int notificationId)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var notification = await _context.Notifications
                    .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);

                if (notification == null)
                    return NotFound();

                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error deleting notification", Error = ex.Message });
            }
        }
    }
}