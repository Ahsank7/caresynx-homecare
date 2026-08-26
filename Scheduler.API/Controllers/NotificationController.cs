using Scheduler.API.Common;
using Scheduler.API.Models.Notification;
using Scheduler.API.Services.Notification;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : BaseController
    {
        private readonly INotification _notification;

        public NotificationController(INotification notification, ILogger<NotificationController> logger)
            : base(logger)
        {
            _notification = notification;
        }

        /// <summary>
        /// Get notifications for the current user
        /// </summary>
        [HttpPost]
        [Route("GetNotifications")]
        public async Task<IActionResult> GetNotifications(NotificationSearchRequest model)
        {
            if (model == null)
                return ValidationError("Search criteria is required");

            // Get user info from JWT token
            var userIdClaim = User.FindFirst("UserID")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return ValidationError("User ID not found in token");

            model.UserId = userId;

            // Get organization and franchise from token if not provided
            if (model.OrganizationId == null)
            {
                var orgIdClaim = User.FindFirst("OrganizationId")?.Value;
                if (!string.IsNullOrEmpty(orgIdClaim) && Guid.TryParse(orgIdClaim, out var orgId))
                    model.OrganizationId = orgId;
            }

            if (model.FranchiseId == null)
            {
                var franchiseIdClaim = User.FindFirst("FranchiseId")?.Value;
                if (!string.IsNullOrEmpty(franchiseIdClaim) && Guid.TryParse(franchiseIdClaim, out var franchiseId))
                    model.FranchiseId = franchiseId;
            }

            if (model.RoleId == null)
            {
                var roleIdClaim = User.FindFirst("RoleId")?.Value;
                if (!string.IsNullOrEmpty(roleIdClaim) && int.TryParse(roleIdClaim, out var roleId))
                    model.RoleId = roleId;
            }

            return await ExecuteAsync(
                () => _notification.GetUserNotificationsAsync(model),
                "Notifications retrieved successfully!"
            );
        }

        /// <summary>
        /// Get unread notification count
        /// </summary>
        [HttpGet]
        [Route("GetUnreadCount")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userIdClaim = User.FindFirst("UserID")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return ValidationError("User ID not found in token");

            Guid? organizationId = null;
            var orgIdClaim = User.FindFirst("OrganizationId")?.Value;
            if (!string.IsNullOrEmpty(orgIdClaim) && Guid.TryParse(orgIdClaim, out var orgId))
                organizationId = orgId;

            Guid? franchiseId = null;
            var franchiseIdClaim = User.FindFirst("FranchiseId")?.Value;
            if (!string.IsNullOrEmpty(franchiseIdClaim) && Guid.TryParse(franchiseIdClaim, out var fId))
                franchiseId = fId;

            int? roleId = null;
            var roleIdClaim = User.FindFirst("RoleId")?.Value;
            if (!string.IsNullOrEmpty(roleIdClaim) && int.TryParse(roleIdClaim, out var rId))
                roleId = rId;

            return await ExecuteAsync(
                async () => new UnreadCountResponse
                {
                    UnreadCount = await _notification.GetUnreadNotificationCountAsync(userId, organizationId, franchiseId, roleId)
                },
                "Unread count retrieved successfully!"
            );
        }

        /// <summary>
        /// Create a new notification (Admin only)
        /// </summary>
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreateNotification(CreateNotificationRequest model)
        {
            if (model == null)
                return ValidationError("Notification data is required");

            if (string.IsNullOrEmpty(model.Title))
                return ValidationError("Title is required");

            if (string.IsNullOrEmpty(model.Message))
                return ValidationError("Message is required");

            var userIdClaim = User.FindFirst("UserID")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return ValidationError("User ID not found in token");

            return await ExecuteAsync(
                () => _notification.CreateNotificationAsync(model, userId),
                "Notification created successfully!"
            );
        }

        /// <summary>
        /// Mark a notification as read
        /// </summary>
        [HttpPost]
        [Route("MarkAsRead")]
        public async Task<IActionResult> MarkAsRead([FromBody] MarkNotificationReadRequest model)
        {
            if (model == null || model.NotificationId == Guid.Empty)
                return ValidationError("Notification ID is required");

            var userIdClaim = User.FindFirst("UserID")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return ValidationError("User ID not found in token");

            model.UserId = userId;

            return await ExecuteAsync(
                () => _notification.MarkNotificationAsReadAsync(model.NotificationId, model.UserId),
                "Notification marked as read!"
            );
        }

        /// <summary>
        /// Mark all notifications as read
        /// </summary>
        [HttpPost]
        [Route("MarkAllAsRead")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userIdClaim = User.FindFirst("UserID")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return ValidationError("User ID not found in token");

            Guid? organizationId = null;
            var orgIdClaim = User.FindFirst("OrganizationId")?.Value;
            if (!string.IsNullOrEmpty(orgIdClaim) && Guid.TryParse(orgIdClaim, out var orgId))
                organizationId = orgId;

            Guid? franchiseId = null;
            var franchiseIdClaim = User.FindFirst("FranchiseId")?.Value;
            if (!string.IsNullOrEmpty(franchiseIdClaim) && Guid.TryParse(franchiseIdClaim, out var fId))
                franchiseId = fId;

            int? roleId = null;
            var roleIdClaim = User.FindFirst("RoleId")?.Value;
            if (!string.IsNullOrEmpty(roleIdClaim) && int.TryParse(roleIdClaim, out var rId))
                roleId = rId;

            return await ExecuteAsync(
                () => _notification.MarkAllNotificationsAsReadAsync(userId, organizationId, franchiseId, roleId),
                "All notifications marked as read!"
            );
        }

        /// <summary>
        /// Delete a notification
        /// </summary>
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> DeleteNotification(Guid notificationId)
        {
            if (notificationId == Guid.Empty)
                return ValidationError("Notification ID is required");

            var userIdClaim = User.FindFirst("UserID")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return ValidationError("User ID not found in token");

            return await ExecuteAsync(
                () => _notification.DeleteNotificationAsync(notificationId, userId),
                "Notification deleted successfully!"
            );
        }
    }
}

