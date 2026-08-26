using Scheduler.API.Models.Address;

namespace Scheduler.API.Services.Address
{
    public interface IAddress
    {
        Task<AddressSearchResponse> GetAddresssAsync(AddressSearchRequest request);
        Task<AddressInfo> GetAddressInfoAsync(Guid AddressID);
        Task<Guid?> CreateUpdateAddressAsync(SaveAddressInfoViewModel saveAddressInfoViewModel);
        Task<Guid?> DeleteAddressAsync(Guid id);
    }
}
