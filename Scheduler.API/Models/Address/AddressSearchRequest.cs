namespace Scheduler.API.Models.Address
{
    public class AddressSearchRequest
    {
        public Guid? UserId { get; set; }
        public string? Address { get; set; }
        public int? AddressTypeId { get; set; }
        public string SortColumn { get; set; }
        public string SortType { get; set; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
