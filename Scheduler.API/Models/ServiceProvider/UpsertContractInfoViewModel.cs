namespace Scheduler.API.Models.ServiceProvider
{
    public class UpsertContractInfoViewModel
    {
        public Guid? Id { get; set; }
        public Guid ServiceProviderUserId { get; set; }
        public int FrequencyId { get; set; }
        public decimal Rate { get; set; }

        public int OptionId { get; set; }
        public int ContractType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
