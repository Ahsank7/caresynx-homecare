namespace Scheduler.API.Models.PlanBoard;

public class UpdateTaskAttendanceRequest
{
    public int TaskId { get; set; }
    public DateTime AttendanceTime { get; set; }
    public Guid? UpdatedBy { get; set; }
}
