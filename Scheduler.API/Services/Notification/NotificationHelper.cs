using Scheduler.API.Models.Notification;

namespace Scheduler.API.Services.Notification
{
    /// <summary>
    /// Helper class for creating activity notifications
    /// </summary>
    public class NotificationHelper
    {
        private readonly INotification _notificationService;

        public NotificationHelper(INotification notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Create a notification for a create activity
        /// </summary>
        public async Task<Guid> NotifyCreateAsync(
            string entityName,
            Guid entityId,
            string entityDisplayName,
            Guid? organizationId,
            Guid? franchiseId,
            Guid createdBy,
            string? actionUrl = null)
        {
            var title = $"New {entityName} Created";
            var message = $"A new {entityName} '{entityDisplayName}' has been created.";

            return await _notificationService.CreateActivityNotificationAsync(
                "Create",
                entityName,
                entityId,
                title,
                message,
                organizationId,
                franchiseId,
                createdBy,
                actionUrl
            );
        }

        /// <summary>
        /// Create a notification for an update activity
        /// </summary>
        public async Task<Guid> NotifyUpdateAsync(
            string entityName,
            Guid entityId,
            string entityDisplayName,
            Guid? organizationId,
            Guid? franchiseId,
            Guid updatedBy,
            string? actionUrl = null)
        {
            var title = $"{entityName} Updated";
            var message = $"The {entityName} '{entityDisplayName}' has been updated.";

            return await _notificationService.CreateActivityNotificationAsync(
                "Update",
                entityName,
                entityId,
                title,
                message,
                organizationId,
                franchiseId,
                updatedBy,
                actionUrl
            );
        }

        /// <summary>
        /// Create a notification for a delete activity
        /// </summary>
        public async Task<Guid> NotifyDeleteAsync(
            string entityName,
            Guid entityId,
            string entityDisplayName,
            Guid? organizationId,
            Guid? franchiseId,
            Guid deletedBy,
            string? actionUrl = null)
        {
            var title = $"{entityName} Deleted";
            var message = $"The {entityName} '{entityDisplayName}' has been deleted.";

            return await _notificationService.CreateActivityNotificationAsync(
                "Delete",
                entityName,
                entityId,
                title,
                message,
                organizationId,
                franchiseId,
                deletedBy,
                actionUrl
            );
        }

        /// <summary>
        /// Create a custom activity notification
        /// </summary>
        public async Task<Guid> NotifyCustomActivityAsync(
            string title,
            string message,
            string activityType,
            string entityName,
            Guid entityId,
            Guid? organizationId,
            Guid? franchiseId,
            Guid createdBy,
            string? actionUrl = null)
        {
            return await _notificationService.CreateActivityNotificationAsync(
                activityType,
                entityName,
                entityId,
                title,
                message,
                organizationId,
                franchiseId,
                createdBy,
                actionUrl
            );
        }

        /// <summary>
        /// Notify about a status change
        /// </summary>
        public async Task<Guid> NotifyStatusChangeAsync(
            string entityName,
            Guid entityId,
            string entityDisplayName,
            string oldStatus,
            string newStatus,
            Guid? organizationId,
            Guid? franchiseId,
            Guid changedBy,
            string? actionUrl = null)
        {
            var title = $"{entityName} Status Changed";
            var message = $"The {entityName} '{entityDisplayName}' status changed from '{oldStatus}' to '{newStatus}'.";

            return await _notificationService.CreateActivityNotificationAsync(
                "StatusChange",
                entityName,
                entityId,
                title,
                message,
                organizationId,
                franchiseId,
                changedBy,
                actionUrl
            );
        }

        /// <summary>
        /// Notify about an assignment
        /// </summary>
        public async Task<Guid> NotifyAssignmentAsync(
            string entityName,
            Guid entityId,
            string entityDisplayName,
            string assignedToName,
            Guid? organizationId,
            Guid? franchiseId,
            Guid assignedBy,
            Guid? assignedToUserId = null,
            string? actionUrl = null)
        {
            var title = $"{entityName} Assigned";
            var message = $"The {entityName} '{entityDisplayName}' has been assigned to {assignedToName}.";

            var notificationRequest = new CreateNotificationRequest
            {
                Title = title,
                Message = message,
                Type = "Activity",
                Priority = 1, // High priority for assignments
                OrganizationId = organizationId,
                FranchiseId = franchiseId,
                TargetUserId = assignedToUserId, // Notify the assigned user specifically
                ActivityType = "Assignment",
                ActivityEntity = entityName,
                ActivityEntityId = entityId,
                ActionUrl = actionUrl
            };

            return await _notificationService.CreateNotificationAsync(notificationRequest, assignedBy);
        }
    }
}

