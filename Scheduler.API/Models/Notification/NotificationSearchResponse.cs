namespace Scheduler.API.Models.Notification
{
    public class NotificationSearchResponse
    {
        public List<NotificationInfo> Notifications { get; set; } = new List<NotificationInfo>();
        public int TotalRecords { get; set; }
        public int UnreadCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}

