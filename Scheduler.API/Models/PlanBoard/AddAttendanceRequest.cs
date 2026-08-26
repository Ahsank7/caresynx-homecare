namespace Scheduler.API.Models.PlanBoard
{
    public class AddAttendanceRequest
    {
        public int TaskId { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
