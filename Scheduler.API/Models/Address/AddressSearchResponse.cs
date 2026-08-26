namespace Scheduler.API.Models.Address
{
    public class AddressSearchResponse
    {
        public List<SearchAddressViewModel> Response { get; set; }
        public int TotalRecords { get; set; }
    }
}
