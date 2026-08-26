using Scheduler.API.Helper;
using Scheduler.API.Models.Lookup;

namespace Scheduler.API.Services.Lookup
{
    public interface ILookup
    {
        Task<LookupResponse> GetLookupAsync(LookupSearchRequest request);
        Task<LookupDetail> GetLookupInfoAsync(int LookupID);
        Task<int?> CreateUpdateLookupAsync(UpsertLookupRequest saveLookupInfoViewModel);
        int? DeleteLookup( int id);
        Task<Dictionary<string, string>> GetLookupsList();
    }
}
