namespace Scheduler.API.Models.Organization
{
    public class OrganizationTimeBasedRate
    {
        public int Id { get; set; }
        public Guid OrganizationId { get; set; }
        public int? ServiceTypeId { get; set; }
        public string? ServiceTypeName { get; set; }
        public int? ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public int DayOfWeek { get; set; }
        public string? DayName { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public decimal ClientRate { get; set; }
        public decimal WageRate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
