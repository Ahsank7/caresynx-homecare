using Scheduler.API.Models.Notification;

namespace Scheduler.API.Services.Notification
{
    public interface INotification
    {
        Task<NotificationSearchResponse> GetUserNotificationsAsync(NotificationSearchRequest request);
        Task<Guid> CreateNotificationAsync(CreateNotificationRequest request, Guid createdBy);
        Task<bool> MarkNotificationAsReadAsync(Guid notificationId, Guid userId);
        Task<int> MarkAllNotificationsAsReadAsync(Guid userId, Guid? organizationId, Guid? franchiseId, int? roleId);
        Task<int> GetUnreadNotificationCountAsync(Guid userId, Guid? organizationId, Guid? franchiseId, int? roleId);
        Task<bool> DeleteNotificationAsync(Guid notificationId, Guid userId);
        
        // Activity tracking helper
        Task<Guid> CreateActivityNotificationAsync(
            string activityType, 
            string activityEntity, 
            Guid activityEntityId, 
            string title, 
            string message, 
            Guid? organizationId, 
            Guid? franchiseId, 
            Guid createdBy,
            string? actionUrl = null
        );
    }
}

