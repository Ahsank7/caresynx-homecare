namespace Scheduler.API.Models.Contact
{
    public class ContactSearchResponse
    {
        public List<SearchContactViewModel> Response { get; set; }
        public int TotalRecords { get; set; }
    }
}
