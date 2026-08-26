namespace Scheduler.API.Models.Scheduler
{
    public class ScheduleAppointmentRequest
    {
        public String ScheduleDescription { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int RecurrencePattern { get; set; }
        public int RecurrenceInterval   { get; set; }
        public String RecurrenceDaysOfWeek { get; set; }
        public String RecurrenceDayOfMonth { get; set; }
        public String RecurrenceDayOfYear { get; set; }

        public int ServiceType { get; set; }
        public String CSVServiceIds { get; set; }
        public Guid ClientId { get; set; }
        public String CSVServiceProviderIds { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid OrganizationId { get; set; }
    }
}
