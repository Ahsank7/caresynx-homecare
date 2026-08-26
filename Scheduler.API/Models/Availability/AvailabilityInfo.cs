namespace Scheduler.API.Models.Availability
{
    public class AvailabilityInfo
    {
        public Guid? Id { get; set; }
        public Guid? UserId { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public string? Day { get; set; }
        public bool IsActive { get; set; }
    }
}
