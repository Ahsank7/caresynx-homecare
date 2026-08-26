using Scheduler.API.Helper;

namespace Scheduler.API.Models.Contact
{
    public class ContactInfo
    {
        public Guid ID { get; set; }
        public Guid UserId { get; set; }
        public string UserNo { get; set; }
        public string? FirstName { get; set; }
        public string? SurName { get; set; }
        public string? LastName { get; set; }
        public string? Alias { get; set; }
        public string? PhoneNo { get; set; }
        public string? MobileNo { get; set; }
        public string? Email { get; set; }
        public string? PassportNo { get; set; }
        public string? IdentityNo { get; set; }
        public int? EthnicityId { get; set; }
        public int Age { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? JoiningDate { get; set; }
        public string? Notes { get; set; }
        public int? TitleId { get; set; }
        public int? GenderId { get; set; }
        public int? NationalityId { get; set; }
        public int? ContactTypeId { get; set; }
        public bool? IsBillingContact { get; set; }
        public int UserType { get; set; }
        public int StatusId { get; set; }
        public bool IsActive { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? AddressLine3 { get; set; }
        public int? CountyId { get; set; }
        public int? StateId { get; set; }
        public int? CountryId { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }
}
