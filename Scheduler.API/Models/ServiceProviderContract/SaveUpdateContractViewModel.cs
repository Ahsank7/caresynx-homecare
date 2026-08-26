namespace Scheduler.API.Models.ServiceProviderContract
{
    public class SaveUpdateContractViewModel
    {
        public Guid ServiceProviderId { get; set; }
        public int ContractTypeId { get; set; }
        public int ExperienceTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; }
    }
}
