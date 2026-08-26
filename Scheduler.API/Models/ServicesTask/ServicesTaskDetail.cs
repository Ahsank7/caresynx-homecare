namespace Scheduler.API.Models.ServicesTask
{
    public class ServicesTaskDetail
    {
        public int TaskId { get; set; }
        public int ScheduleId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime Date { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public DateTime? CheckInTime { get; set; }
        public Guid ClientId { get; set; }
        public string? ClientUserNo { get; set; }
        public string? ClientName { get; set; }
        public string? ClientEmail { get; set; }
        public string? ClientPhone { get; set; }
        public string? ClientMobile { get; set; }
        public string? ClientAddress { get; set; }
        public Guid ServiceProviderId { get; set; }
        public string? ServiceProviderUserNo { get; set; }
        public int Status { get; set; }
        public string? ServiceProviderName { get; set; }
        public string? ServiceProviderEmail { get; set; }
        public string? ServiceProviderPhone { get; set; }
        public string? ServiceProviderMobile { get; set; }
        public string? ServiceProviderAddress { get; set; }
        public bool IsConfirmed { get; set; }
        public string? FranchiseName { get; set; }
        public string? TaskStatus { get; set; }
        public string? ServiceType { get; set; }
        public string? ServiceName { get; set; }
    }
}
