namespace Scheduler.API.Models.ToConfirm
{
    public class ToConfirmResponse
    {
        public List<ToConfirmDetail> Response { get; set; }
        public int TotalRecords { get; set; }
    }
}
