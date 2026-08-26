namespace Scheduler.API.Models.Wage
{
    public class GenerateWageResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int GeneratedWages { get; set; }
    }
} 