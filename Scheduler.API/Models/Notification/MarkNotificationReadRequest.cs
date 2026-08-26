namespace Scheduler.API.Models.Notification
{
    public class MarkNotificationReadRequest
    {
        public Guid NotificationId { get; set; }
        public Guid UserId { get; set; }
    }
}

