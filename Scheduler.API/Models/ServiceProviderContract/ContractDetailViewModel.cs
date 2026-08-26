namespace Scheduler.API.Models.ServiceProviderContract
{
    public class ContractDetailViewModel
    {
        public Guid ServiceProviderId { get; set; }
        public string? ContractType { get; set; }
        public string? ExperienceType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string WageChart { get; set; }
    }
}
