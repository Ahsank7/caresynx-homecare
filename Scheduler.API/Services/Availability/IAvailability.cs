using Scheduler.API.Models.Availability;

namespace Scheduler.API.Services.Availability
{
    public interface IAvailability
    {
        Task<AvailabilitySearchResponse> GetAvailabilitysAsync(AvailabilitySearchRequest request);
        Task<AvailabilityInfo> GetAvailabilityInfoAsync(Guid AvailabilityID);
        Task<Guid?> CreateUpdateAvailabilityAsync(SaveAvailabilityInfoViewModel saveAvailabilityInfoViewModel);
        Guid? DeleteAvailability(Guid id);
    }
}
