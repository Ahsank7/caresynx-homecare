namespace Scheduler.API.Models.Service
{
    public class DeleteEntityResult
    {
        public bool Deleted { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
