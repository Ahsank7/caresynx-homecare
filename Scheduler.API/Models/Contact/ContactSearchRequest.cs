namespace Scheduler.API.Models.Contact
{
    public class ContactSearchRequest
    {
        public Guid? UserId { get; set; }
        public Guid? ContactUserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? MobileNumber { get; set; }
        public string? Email { get; set; }
        public int? GenderId { get; set; }
        public int? StatusId { get; set; }
        public int? EthnicityId { get; set; }
        public int? ContactTypeId { get; set; }
        public string SortColumn { get; set; }
        public string SortType { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }


    }
}
