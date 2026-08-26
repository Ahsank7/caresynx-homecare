namespace Scheduler.API.Models.Wage
{
    public class GenerateWageRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid? OrganizationId { get; set; }
    }
} 