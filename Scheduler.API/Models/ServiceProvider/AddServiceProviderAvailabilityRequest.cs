namespace Scheduler.API.Models.ServiceProvider
{
    public class AddServiceProviderAvailabilityRequest
    {
        public String AvailableDays { get; set; }
        public DateTime StartDate {  get; set; }
        public DateTime EndDate { get; set; }
        public Guid ServiceProviderId { get; set; }
    }
}
